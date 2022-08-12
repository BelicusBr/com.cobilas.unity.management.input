namespace Cobilas.Unity.Management.InputManager.ALFCIC {
    internal class CIMCICComment : CIMCICStream {
        private string value;
        private CIMCICStream parent;
        private const string name = "Comment";

        public override string Name => name;

        public override CIMCICStream Parent {
            get => parent;
            set => parent = value;
        }

        internal CIMCICComment(CIMCICStream parent, string value) {
            this.value = value;
            this.parent = parent;
        }

        internal CIMCICComment(string value)
          : this((CIMCICStream)null, value) { }

        public override void Dispose() {
            value = (string)null;
            parent = (CIMCICStream)null;
        }

        public override string ToString() 
            => string.Format("#* value:{0}", this.value);
    }
}
