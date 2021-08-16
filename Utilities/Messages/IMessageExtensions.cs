using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class IMessageExtensions
    {
        /// <summary>
        /// Takes an <see cref="IEnumerable{T}"/> set of <see cref="IMessage"/>s and prints them as multi-line string.
        /// </summary>
        /// <param name="msgs"></param>
        /// <returns> A string representation of the <paramref name="msgs"/>, with each message printed on its own line in the format: [<see cref="IMessage.Level"/>] <see cref="IMessage.Notice"/> </returns>
        public static string PrintTabbedListOfMessages(this IEnumerable<IMessage> msgs)
        {
            string msg = "";
            foreach (var m in msgs) msg += $"\n\t[{m.Level}] {m.Notice}";
            return msg;
        }
        /// <summary>
        /// Evaluates the <paramref name="msgs"/> providing the most severe <see cref="IMessage.Level"/> in the group of messages.
        /// </summary>
        /// <param name="msgs"> An <see cref="IEnumerable{T}"/> set of <see cref="IMessage"/>s. </param>
        /// <returns> The most severe <see cref="IMessage.Level"/> in <paramref name="msgs"/>. </returns>
        public static Utilities.IMessageLevels Max(this IEnumerable<IMessage> msgs)
        {
            Utilities.IMessageLevels level = Utilities.IMessageLevels.NoErrors;
            foreach (Utilities.IMessage msg in msgs)
            {
                if (msg.Level == Utilities.IMessageLevels.FatalError) return IMessageLevels.FatalError;
                if (msg.Level > level) level = msg.Level;
            }
            return level;
        }
    }
}
