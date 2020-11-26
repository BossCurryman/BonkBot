using System;
using System.Collections.Generic;
using System.Text;

namespace BonkBot
{
    class UserValues
    {
        public Authority Auth { get; set; }
        public uint TimesBonked { get; set; }
        public uint TimesBonking { get; set; }

        public UserValues()
        {
        }

        public UserValues(Authority auth = Authority.BonkGuard)
        {
            Auth = auth;
            TimesBonked = 0;
            TimesBonking = 0;
        }
    }
}
