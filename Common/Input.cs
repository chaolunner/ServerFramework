using System;

namespace Common
{
    [Serializable]
    public class Input
    {
        public int Step;
        public static Input None = new Input() { Step = -1 };

        public override int GetHashCode()
        {
            return Step.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Input && ((Input)obj) == this;
        }

        public static bool operator ==(Input currentInput, Input anotherInput)
        {
            return currentInput.Step == anotherInput.Step;
        }

        public static bool operator !=(Input currentInput, Input anotherInput)
        {
            return currentInput.Step != anotherInput.Step;
        }
    }
}
