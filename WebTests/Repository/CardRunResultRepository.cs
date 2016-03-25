using System;
using System.Collections.Generic;
using System.Linq;
using DBreeze;
using WebTests.Models;

namespace WebTests.Repository
{
    public class CardRunResultRepository : ICardRunResultRepository
    {
        private readonly DBreezeEngine _dbEngine;
        private const string TABLE_NAME = "CARD_RUN_RESULTS";

        public CardRunResultRepository(Options options)
        {
            _dbEngine = new DBreezeEngine(options.DbLocation);
            DBreeze.Utils.CustomSerializator.ByteArraySerializator = ProtobufSerializers.SerializeProtobuf;
            DBreeze.Utils.CustomSerializator.ByteArrayDeSerializator = ProtobufSerializers.DeserializeProtobuf;
        }

        public OperationResult Add(CardRunResult cardRunResult)
        {
            this.EnsureRecordHasId(cardRunResult);

            using(var trans = _dbEngine.GetTransaction())
            {
                trans.Insert<long, CardRunResult>(TABLE_NAME, cardRunResult.Id, cardRunResult);
                trans.Commit();
            }

            return OperationResult.Upsert;
        }

        public OperationResult Add(IEnumerable<CardRunResult> cardRunResults)
        {
            using(var trans = _dbEngine.GetTransaction())
            {
                foreach(var results in cardRunResults)
                {
                    var currentRecord = trans.Select<long, CardRunResult>(TABLE_NAME, results.Id);

                    if(!currentRecord.Exists)
                    {
                        trans.Insert<long, CardRunResult>(TABLE_NAME, results.Id, results);
                    }
                }
                trans.Commit();
            }

            return OperationResult.Add;
        }
        
        public void ExportRecords(Action<IEnumerable<CardRunResult>> exporter)
        {
            using(var trans = _dbEngine.GetTransaction())
            {
                var records =
                    from rows in trans.SelectForward<long, CardRunResult>(TABLE_NAME)
                    select rows.Value;
                exporter(records);
            }
        }

        private void EnsureRecordHasId(CardRunResult cardRunResult)
        {
            if(cardRunResult.Id > 0)
            {
                return;
            }

            var nextId = this.GetLastCardResultId();
            cardRunResult.Id = nextId + 1;
        }

        private long GetLastCardResultId()
        {
            using(var trans = _dbEngine.GetTransaction())
            {
                var lastRunResult = trans.SelectBackward<long, CardRunResult>(TABLE_NAME).FirstOrDefault();

                return lastRunResult?.Key ?? -1;
            }
        }
    }
}