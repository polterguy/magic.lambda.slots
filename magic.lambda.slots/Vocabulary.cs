﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using System.Collections.Generic;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.slots
{
    /// <summary>
    /// [slots.vocabulary] slot that will return the names of all dynamically created slots to caller.
    /// </summary>
    [Slot(Name = "slots.vocabulary")]
    public class Vocabulary : ISlot
    {
        /// <summary>
        /// Slot implementation.
        /// </summary>
        /// <param name="signaler">Signaler that raised signal.</param>
        /// <param name="input">Arguments to slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Retrieving slot's lambda, no reasons to clone, GetSlot will clone.
            var filter = input.GetEx<string>();
            input.Value = null;
            if (string.IsNullOrEmpty(filter))
            {
                var list = Create.Slots()
                    .Select(x => new Node("", x))
                    .ToList();
                list.Sort((lhs, rhs) => string
                    .Compare(
                        lhs.Get<string>(),
                        rhs.Get<string>(),
                        System.StringComparison.InvariantCulture));
                var whitelist = signaler.Peek<List<Node>>("whitelist");
                input.AddRange(list
                    .Where(x => whitelist == null ||
                        whitelist.Any(x2 => x2.Name == "signal" && x2.Get<string>() == x.Get<string>())));
            }
            else
            {
                var list = Create.Slots()
                    .Where(x => x.StartsWith(filter))
                    .Select(x => new Node("", x))
                    .ToList();
                list.Sort((lhs, rhs) => string
                    .Compare(
                        lhs.Get<string>(),
                        rhs.Get<string>(),
                        System.StringComparison.InvariantCulture));
                var whitelist = signaler.Peek<List<Node>>("whitelist");
                input.AddRange(list.Where(x => whitelist == null ||
                    whitelist.Any(x2 => x2.Name == "signal" && x2.Get<string>() == x.Get<string>())));
            }
        }
    }
}
