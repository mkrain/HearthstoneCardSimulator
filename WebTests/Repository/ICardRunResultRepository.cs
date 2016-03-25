using System;
using System.Collections.Generic;
using WebTests.Models;

namespace WebTests.Repository
{
    public interface ICardRunResultRepository
    {
        OperationResult Add(CardRunResult cardRunResult);
        OperationResult Add(IEnumerable<CardRunResult> cardRunResult);
        void ExportRecords(Action<IEnumerable<CardRunResult>> exporter);
    }

    public enum OperationResult
    {
        None = 0,
        Insert,
        Add,
        Update,
        Upsert,
        Delete
    }
}