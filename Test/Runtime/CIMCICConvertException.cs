using System;

namespace Cobilas.Unity.Management.InputManager.ALFCIC {
    [Serializable]
    public class CIMCICConvertException : Exception {
        public CIMCICConvertException() { }
        public CIMCICConvertException(string message) : base(message) { }
        public CIMCICConvertException(string message, Exception inner) : base(message, inner) { }
        protected CIMCICConvertException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}