using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CsvHelper;
using WebTests.Models;
using WebTests.Repository;

namespace WebTests.Services
{
    public class FileService : IFileService, IDisposable
    {
        private readonly Options _options;
        private readonly IMapper _mapper;
        private readonly ICardRunResultRepository _runRepository;
        private readonly CsvWriter _csvWriter;

        public FileService(ICardRunResultRepository repository, IMapper mapper, Options options)
        {
            _options = options;
            _mapper = mapper;
            _runRepository = repository;

            if(!Directory.Exists(_options.DataFilesDirectory))
            {
                Directory.CreateDirectory(_options.DataFilesDirectory);
            }

            if(options.WriteCsv)
            {
                var fileName = this.GetFileSave(string.Concat(options.LogFileName, DateTime.Now.ToString("yyyyMMddHHmmss"), ".csv"));
                var file = File.CreateText(fileName);
                file.AutoFlush = true;
                _csvWriter = new CsvWriter(file);
            }

            if(!options.Export)
            {
                return;
            }

            var exportFileName = this.GetFileSave(_options.ExportFileName);
            using(var csvExporter = new CsvWriter(File.CreateText(exportFileName)))
            {
                var exporter = csvExporter;

                _runRepository.ExportRecords(
                    records =>
                    {
                        var mapped =
                            from r in records
                            select _mapper.Map<CardRunResultMap>(r);

                        exporter.WriteRecords(mapped.SelectMany(cr => cr.Records));
                    });
            }
        }

        public void MergeExistingRunResults()
        {
            var mergeFile = this.GetMergeFile();

            if(!File.Exists(mergeFile))
            {
                return;
            }

            var cardRunResults = new List<CardRunResult>();

            //Read merge file and write to DB if it's missing records
            using(var stream = File.OpenText(mergeFile))
            {
                using(var csv = new CsvReader(stream))
                {
                    var records = csv.GetRecords<CardRunResultRecord>();

                    var grouped = records.GroupBy(r => r.Id, r => r);

                    //Group the cards by runId
                    foreach(var runResult in grouped)
                    {
                        CardRunResult result;

                        var allCards = runResult.ToList();

                        //Only 5 cards exist per run
                        if(allCards.Count > 5)
                        {
                            var cardRecord = allCards.GroupBy(c => c.CardId).ToList();
                            var distinctCards = cardRecord.SelectMany(c => c).Distinct();
                            result = _mapper.Map<CardRunResult>(distinctCards.Take(5));
                        }
                        else
                        {
                            result = _mapper.Map<CardRunResult>(allCards);
                        }

                        cardRunResults.Add(result);
                    }
                }
            }

            var oldFile = this.GetFileSave("merge.old.csv");
            //Rename merge.csv
            if(File.Exists(oldFile))
            {
                File.Delete(oldFile);
            }

            File.Copy(mergeFile, oldFile);

            var operation = _runRepository.Add(cardRunResults);

            //Only delete the file if the database operation succeeded.
            if(operation == OperationResult.Add)
            {
                File.Delete(mergeFile);
            }
        }

        public OperationResult SaveRuResult(CardRunResult runResult)
        {
            if(_options.WriteCsv && _csvWriter != null)
            {
                var records = _mapper.Map<CardRunResultMap>(runResult);
                _csvWriter.WriteRecords(records.Records);
            }

            var operationResult = _runRepository.Add(runResult);

            return operationResult;
        }

        private string GetMergeFile()
        {
            var pathWithFile = Path.Combine(_options.DataFilesDirectory, string.Concat(_options.MergeFileName, ".csv"));

            return pathWithFile;
        }

        private string GetFileSave(string fileName)
        {
            var pathWithFile = Path.Combine(_options.DataFilesDirectory, fileName);

            return pathWithFile;
        }

        private void Dispose(bool disposing)
        {
            if(!disposing)
            {
                return;
            }

            _csvWriter?.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}