using System;
using System.Collections.Generic;
using System.Text;

using AsapNotificationSystem.DataBase.Models;

namespace AsapNotificationSystem.Convert
{
    public static class BnToString
    {
        private static Dictionary<BuildingNumber, string> bnToStringDictionary = new Dictionary<BuildingNumber, string> {
            { BuildingNumber.One           , "1"    },
            { BuildingNumber.OnePointEight , "1.8"  },
            { BuildingNumber.OnePointNine  , "1.9"  },
            { BuildingNumber.OnePointTen   , "1.10" },
            { BuildingNumber.Two           , "2"    },
            { BuildingNumber.TwoPointOne   , "2.1"  },
            { BuildingNumber.TwoPointTwo   , "2.2"  },
            { BuildingNumber.TwoPointThree , "2.3"  },
            { BuildingNumber.TwoPointFour  , "2.4"  },
            { BuildingNumber.TwoPointFive  , "2.5"  },
            { BuildingNumber.TwoPointSix   , "2.6"  },
            { BuildingNumber.TwoPointSeven , "2.7"  },
            { BuildingNumber.Three         , "3"    },
            { BuildingNumber.Four          , "4"    },
            { BuildingNumber.Five          , "5"    },
            { BuildingNumber.SixPointOne   , "6.1"  },
            { BuildingNumber.SixPointTwo   , "6.2"  },
            { BuildingNumber.SevenPointOne , "7.1"  },
            { BuildingNumber.SevenPointTwo , "7.2"  },
            { BuildingNumber.EightPointOne , "8.1"  },
            { BuildingNumber.EightPointTwo , "8.2"  },
            { BuildingNumber.Nine          , "9"    },
            { BuildingNumber.Ten           , "10"   },
            { BuildingNumber.Eleven        , "11"   },
            { BuildingNumber.A             , "A"    },
            { BuildingNumber.B             , "B"    },
            { BuildingNumber.C             , "C"    },
            { BuildingNumber.D             , "D"    },
            { BuildingNumber.E             , "E"    },
            { BuildingNumber.F             , "F"    },
            { BuildingNumber.G             , "G"    },
            { BuildingNumber.L             , "L"    },
            { BuildingNumber.M             , "M"    },
            { BuildingNumber.SOne          , "S1"   },
            { BuildingNumber.STwo          , "S2"   },
            { BuildingNumber.S             , "S"    }
        };

        public static string Convert(BuildingNumber bn) => bnToStringDictionary.ContainsKey(bn) ? bnToStringDictionary[bn] : "other";
    }
}
