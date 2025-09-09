using System;
using network.client.ultis;

namespace network.client.message {
    public class InsufficientCapacityException : Exception {
        public readonly Message _message = new Message();

        public readonly string TypeName = "";

        public readonly int RequiredBytes;

        public InsufficientCapacityException() {
        }

        public InsufficientCapacityException(string message)
            : base(message) {
        }

        public InsufficientCapacityException(string message, Exception inner)
            : base(message, inner) {
        }

        public InsufficientCapacityException(Message message, string typeName, int requiredBytes)
            : base(GetErrorMessage(message, typeName, requiredBytes)) {
            _message = message;
            RequiredBytes = requiredBytes;
            TypeName = typeName;
        }

        public InsufficientCapacityException(Message message, int arrayLength, string typeName, int requiredBytes, int totalRequiredBytes = -1)
            : base(GetErrorMessage(message, arrayLength, typeName, requiredBytes, totalRequiredBytes)) {
            _message = message;
            RequiredBytes = totalRequiredBytes == -1 ? arrayLength * requiredBytes : totalRequiredBytes;
            TypeName = typeName + "[]";
        }

        private static string GetErrorMessage(Message message, string typeName, int requiredBytes) {
            return string.Format("Cannot add a value of type '{0}' (requires {1} {2}) to ", typeName, requiredBytes, Helper.CorrectForm(requiredBytes, "byte")) + string.Format("a message with {0} {1} of remaining capacity!", message.UnwrittenLength, Helper.CorrectForm(message.UnwrittenLength, "byte"));
        }

        private static string GetErrorMessage(Message message, int arrayLength, string typeName, int requiredBytes, int totalRequiredBytes) {
            if (totalRequiredBytes == -1) totalRequiredBytes = arrayLength * requiredBytes;

            return string.Format("Cannot add an array of type '{0}[]' with {1} {2} (requires {3} {4}) ", typeName, arrayLength, Helper.CorrectForm(arrayLength, "element"), totalRequiredBytes, Helper.CorrectForm(totalRequiredBytes, "byte")) + string.Format("to a message with {0} {1} of remaining capacity!", message.UnwrittenLength, Helper.CorrectForm(message.UnwrittenLength, "byte"));
        }
    }

}
