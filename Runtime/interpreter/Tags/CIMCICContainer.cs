namespace Cobilas.Unity.Management.InputManager.ALFCIC {
    internal class CIMCICContainer : CIMCICStream {
        private string name;
        private string type;
        private string value;
        private CIMCICStream parent;

        public string Type => type;
        public string Value => value;
        public override string Name => name;

        public override CIMCICStream Parent {
            get => parent;
            set => parent = value;
        }

        internal CIMCICContainer(CIMCICStream parent, string name, string type, string value) {
            this.name = name;
            this.type = type;
            this.value = value;
            this.parent = parent;
        }

        internal CIMCICContainer(string name, string type, string value)
          : this((CIMCICStream)null, name, type, value) { }

        public override void Dispose() {
            parent = (CIMCICStream)null;
            name = type = value = (string)null;
        }

        public override string ToString() 
            => string.Format("{0} name:{1} [{2}]", this.type, this.name, this.value);
    }
}
