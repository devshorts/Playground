using DecisionTree.Data;

namespace SampleDataParsers
{
    public interface IParser
    {
        DecisionTreeSet Parse(string file);
    }
}
