using System;

namespace Cobilas.Unity.Management.InputManager {
    internal sealed class InputCapsuleResult : IDisposable {
        private uint mark_TriggerFirst;
        private uint mark_SecondaryTrigger;
        private string ID_Target;
        private bool result;

        public bool Result => result;
        public string IDTarget => ID_Target;
        public uint Mark_TriggerFirst => mark_TriggerFirst;
        public uint Mark_SecondaryTrigger => mark_SecondaryTrigger;

        public void Confirm() => result = true;

        public void MarkTriggerFirst() => ++mark_TriggerFirst;

        public void MarkSecondaryTrigger() => ++mark_SecondaryTrigger;

        public InputCapsuleResult SetID(string IDTarget) {
            ID_Target = IDTarget;
            return this;
        }

        public void Dispose() {
            result = false;
            mark_SecondaryTrigger = mark_TriggerFirst = 0;
        }
    }
}
