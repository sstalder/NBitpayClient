using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NBitpayClient
{
    /// <summary>
    /// Provides an API specific exception handler.
    /// </summary>
    public class BitPayException : Exception
    {
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Constructor.  Creates an empty exception.
        /// </summary>
        public BitPayException()
        {
        }

        /// <summary>
        /// Constructor.  Creates an exception with a message only.
        /// </summary>
        /// <param name="message">The message text for the exception.</param>
        public BitPayException(string message)
        {
            if (message != null && message.StartsWith("{\"error\""))
            {
                var item = JsonConvert.DeserializeObject<BitPayError>(message, new JsonSerializerSettings
                {
                    Error = (sender, args) => args.ErrorContext.Handled = true
                });

                if (item != null)
                {
                    AddError(item.Error);
                }
            }
            else
            {
                AddError(message);
            }
        }

        /// <summary>
        /// Constructor.  Creates an exception with a message and root cause exception.
        /// </summary>
        /// <param name="message">The message text for the exception.</param>
        /// <param name="inner">The root cause exception.</param>
        public BitPayException(string message, Exception inner) : base(message, inner)
        {
            AddError(message);
        }

        public void AddError(string message)
        {
            Errors.Add(message);
        }

        public override string ToString()
        {
            var output = base.ToString();

            if (Errors.Any())
            {
                output = "BitPay Errors: " + string.Join(Environment.NewLine, Errors) + " -> " + output;
            }

            return output;
        }
    }
}