namespace Logging
{
    public interface ILogger
    {
        bool AutoFlush { get; set; }

        void Write(object message);
        void Write(string message);
        void Write(string format, params object[] parameters);
        
        void WriteLine(object message);
        void WriteLine(string message);
        void WriteLine(string format, params object[] parameters);

        void Flush();
    }
}
