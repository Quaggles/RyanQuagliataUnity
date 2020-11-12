using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace QFSW.QC
{
    /// <summary>
    /// Creates a prefix that will be prepended to all commands made within this class. Works recursively with sub-classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public sealed class CommandPrefixAttribute : Attribute
    {
        public readonly string Prefix;
        public readonly bool Valid = true;

        private static readonly char[] _bannedAliasChars = new char[] { ' ', '(', ')', '{', '}', '[', ']', '<', '>' };

        public CommandPrefixAttribute([CallerMemberName] string prefixName = "")
        {
            Prefix = prefixName;
            for (int i = 0; i < _bannedAliasChars.Length; i++)
            {
                if (Prefix.Contains(_bannedAliasChars[i]))
                {
                    string errorMessage = $"Development Processor Error: Command prefix '{Prefix}' contains the char '{_bannedAliasChars[i]}' which is banned. Unexpected behaviour may occurr.";
                    Debug.LogError(errorMessage);

                    Valid = false;
                    throw new ArgumentException(errorMessage, nameof(prefixName));
                }
            }
        }
    }
}
