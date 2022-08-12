using System;
using System.Text;
using System.Collections;
using Cobilas.Collections;
using System.Collections.Generic;

namespace Cobilas.Unity.Management.InputManager.ALFCIC {
    internal class CIMCICTag : CIMCICStream, IEnumerable<CIMCICStream> {
        private string name;
        private string type;
        private CIMCICStream parent;
        private CIMCICStream[] streams;

        public string Type => type;
        public override string Name => name;
        public int Count => ArrayManipulation.ArrayLength(streams);

        public override CIMCICStream Parent {
            get => parent;
            set => parent = value;
        }

        public CIMCICStream this[int index] => streams[index];

        internal CIMCICTag(CIMCICTag parent, string name, string type, params CIMCICStream[] streams) {
            this.name = name;
            this.type = type;
            this.streams = streams;
            this.parent = (CIMCICStream)parent;
        }

        internal CIMCICTag(CIMCICTag parent, string name, string type)
          : this(parent, name, type, (CIMCICStream[])null) { }

        internal CIMCICTag(string name, string type, params CIMCICStream[] streams)
          : this((CIMCICTag)null, name, type, streams) { }

        internal CIMCICTag(string name, string type)
          : this(name, type, (CIMCICStream[])null) { }

        public void Add(CIMCICStream stream) {
            stream.Parent = (CIMCICStream)this;
            ArrayManipulation.Add<CIMCICStream>(stream, ref streams);
        }

        public string GetValueFromValueFlag(string name) {
            for (int index = 0; index < Count; ++index)
                if (streams[index] is CIMCICContainer stream1 && stream1.Type == "#&" && stream1.Name == name)
                    return stream1.Value;
            return (string)null;
        }

        public override void Dispose() {
            for (int index = 0; index < Count; ++index)
                streams[index].Dispose();

            name = type = (string)null;
            parent = (CIMCICStream)null;
            ArrayManipulation.ClearArraySafe<CIMCICStream>(ref streams);
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            ToString(builder, string.Empty);
            return builder.ToString();
        }

        public IEnumerator<CIMCICStream> GetEnumerator() 
            => new ArrayToIEnumerator<CIMCICStream>(GetList());

        private void ToString(StringBuilder builder, string step) {
            builder.AppendFormat("{0}{1} name:{2} [{3}]\r\n", step, type, name, Count);
            for (int index = 0; index < Count; ++index) {
                if (streams[index] is CIMCICTag tag) tag.ToString(builder, string.Format("\t{0}", step));
                else builder.AppendFormat("\t{0}{1}\r\n", step, streams[index]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() 
            => new ArrayToIEnumerator<CIMCICStream>(GetList());

        private CIMCICStream[] GetList() => Count <= 0 ? new CIMCICStream[0] : streams;
    }
}
