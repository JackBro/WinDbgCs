﻿using CsDebugScript.Exceptions;
using System;

namespace CsDebugScript.CommonUserTypes.NativeTypes.std
{
    /// <summary>
    /// Microsoft implementation of std::basic_string
    /// </summary>
    public class basic_string
    {
        private interface IBasicString
        {
            /// <summary>
            /// Gets the string length.
            /// </summary>
            int Length { get; }

            /// <summary>
            /// Gets the reserved space in buffer (number of characters).
            /// </summary>
            int Reserved { get; }

            /// <summary>
            /// Gets the string text.
            /// </summary>
            string Text { get; }
        }

        /// <summary>
        /// Common code for Microsoft Visual Studio implementations of std::basic_string
        /// </summary>
        public class VisualStudio : IBasicString
        {
            /// <summary>
            /// The internal value field inside the std::basic_string
            /// </summary>
            private UserMember<Variable> value;

            /// <summary>
            /// The length of the string
            /// </summary>
            private UserMember<int> length;

            /// <summary>
            /// The reserved number of characters in the string buffer
            /// </summary>
            private UserMember<int> reserved;

            /// <summary>
            /// The text inside the string buffer
            /// </summary>
            private UserMember<string> text;

            /// <summary>
            /// Initializes a new instance of the <see cref="VisualStudio"/> class.
            /// </summary>
            /// <param name="value">The value variable.</param>
            public VisualStudio(UserMember<Variable> value)
            {
                this.value = value;
                length = UserMember.Create(() => (int)Value.GetField("_Mysize"));
                reserved = UserMember.Create(() => (int)Value.GetField("_Myres"));
                text = UserMember.Create(GetText);
            }

            /// <summary>
            /// Gets the string length.
            /// </summary>
            public int Length
            {
                get
                {
                    return length.Value;
                }
            }

            /// <summary>
            /// Gets the reserved space in buffer (number of characters).
            /// </summary>
            public int Reserved
            {
                get
                {
                    return reserved.Value;
                }
            }

            /// <summary>
            /// Gets the string text.
            /// </summary>
            public string Text
            {
                get
                {
                    return text.Value;
                }
            }

            /// <summary>
            /// Gets the internal value field inside the std::basic_string
            /// </summary>
            private Variable Value
            {
                get
                {
                    return value.Value;
                }
            }

            /// <summary>
            /// Gets the string text.
            /// </summary>
            private string GetText()
            {
                var bx = Value.GetField("_Bx");
                var buf = bx.GetField("_Buf");

                if (Length >= buf.GetArrayLength())
                {
                    return bx.GetField("_Ptr").ToString();
                }
                else
                {
                    return buf.ToString();
                }
            }
        }

        /// <summary>
        /// Microsoft Visual Studio 2013 implementation of std::basic_string
        /// </summary>
        public class VisualStudio2013 : VisualStudio
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="VisualStudio2013"/> class.
            /// </summary>
            /// <param name="variable">The variable.</param>
            public VisualStudio2013(Variable variable)
                : base(UserMember.Create(() => variable))
            {
            }

            /// <summary>
            /// Verifies if the specified code type is correct for this class.
            /// </summary>
            /// <param name="codeType">The code type.</param>
            internal static bool VerifyCodeType(CodeType codeType)
            {
                // We want to have this kind of hierarchy
                // _Bx
                // | _Buf
                // | _Ptr
                // _Mysize
                // _Myres
                CodeType _Bx, _Buf, _Ptr, _Mysize, _Myres;
                var fields = codeType.GetFieldTypes();

                if (!fields.TryGetValue("_Bx", out _Bx) || !fields.TryGetValue("_Mysize", out _Mysize) || !fields.TryGetValue("_Myres", out _Myres))
                    return false;

                var _BxFields = _Bx.GetFieldTypes();

                if (!_BxFields.TryGetValue("_Buf", out _Buf) || !_BxFields.TryGetValue("_Ptr", out _Ptr))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Microsoft Visual Studio 2015 implementation of std::basic_string
        /// </summary>
        public class VisualStudio2015 : VisualStudio
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="VisualStudio2015"/> class.
            /// </summary>
            /// <param name="variable">The variable.</param>
            public VisualStudio2015(Variable variable)
                : base(UserMember.Create(() => variable.GetField("_Mypair").GetField("_Myval2")))
            {
            }

            /// <summary>
            /// Verifies if the specified code type is correct for this class.
            /// </summary>
            /// <param name="codeType">The code type.</param>
            internal static bool VerifyCodeType(CodeType codeType)
            {
                // We want to have this kind of hierarchy
                // _Mypair
                // | _Myval2
                //   | _Bx
                //     | _Buf
                //     | _Ptr
                //   | _Mysize
                //   | _Myres
                CodeType _Mypair, _Myval2, _Bx, _Buf, _Ptr, _Mysize, _Myres;

                if (!codeType.GetFieldTypes().TryGetValue("_Mypair", out _Mypair))
                    return false;
                if (!_Mypair.GetFieldTypes().TryGetValue("_Myval2", out _Myval2))
                    return false;

                var _Myval2Fields = _Myval2.GetFieldTypes();

                if (!_Myval2Fields.TryGetValue("_Bx", out _Bx) || !_Myval2Fields.TryGetValue("_Mysize", out _Mysize) || !_Myval2Fields.TryGetValue("_Myres", out _Myres))
                    return false;

                var _BxFields = _Bx.GetFieldTypes();

                if (!_BxFields.TryGetValue("_Buf", out _Buf) || !_BxFields.TryGetValue("_Ptr", out _Ptr))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// The type selector
        /// </summary>
        private static TypeSelector<IBasicString> typeSelector = new TypeSelector<IBasicString>(new[]
        {
            new Tuple<Type, Func<CodeType, bool>>(typeof(VisualStudio2013), VisualStudio2013.VerifyCodeType),
            new Tuple<Type, Func<CodeType, bool>>(typeof(VisualStudio2015), VisualStudio2015.VerifyCodeType),
        });

        /// <summary>
        /// The instance used to read variable data
        /// </summary>
        private IBasicString instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="basic_string"/> class.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <exception cref="WrongCodeTypeException">std::basic_string</exception>
        public basic_string(Variable variable)
        {
            instance = typeSelector.SelectType(variable);
            if (instance == null)
            {
                throw new WrongCodeTypeException(variable, nameof(variable), "std::basic_string");
            }
        }

        /// <summary>
        /// Gets the string length.
        /// </summary>
        public int Length
        {
            get
            {
                return instance.Length;
            }
        }

        /// <summary>
        /// Gets the reserved space in buffer (number of characters).
        /// </summary>
        public int Reserved
        {
            get
            {
                return instance.Reserved;
            }
        }

        /// <summary>
        /// Gets the string text.
        /// </summary>
        public string Text
        {
            get
            {
                return instance.Text;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return Text;
        }
    }
}
