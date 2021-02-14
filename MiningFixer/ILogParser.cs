using System;

namespace MiningFixer
{
    public interface ILogParser : IDisposable
    {
        void Parse();
    }
}
