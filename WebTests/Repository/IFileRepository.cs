using WebTests.Models;

namespace WebTests.Repository
{
    public interface  IFileService
    {
        void MergeExistingRunResults();
        OperationResult SaveRuResult(CardRunResult runResult);
    }
}