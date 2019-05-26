using BitMexLibrary.Enums;

namespace BitMexLibrary
{

    public partial class STR
    {

        // Делегат
        public delegate void SignalHandler(SignalEnum signal);
        // Событие
        public event SignalHandler SignalEvent;
        // Метод для вызова события
        public void OnSignal(SignalEnum signal) => SignalEvent?.Invoke(signal);
    }
}
