using System;

namespace Cobilas.Unity.Management.InputManager.ALFCIC {
    internal abstract class CIMCICStream : IDisposable {
        public abstract string Name { get; }

        public abstract CIMCICStream Parent { get; set; }

        public abstract void Dispose();
    }
}
