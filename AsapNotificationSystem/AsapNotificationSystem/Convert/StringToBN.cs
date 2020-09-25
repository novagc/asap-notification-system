using System;
using System.Collections.Generic;
using System.Text;
using AsapNotificationSystem.DataBase.Models;

namespace AsapNotificationSystem.Convert
{
    public static class StringToBnConverter
    {
        private static Dictionary<string, BuildingNumber> bnDictionary = new Dictionary<string, BuildingNumber>
        {
            { "1"   , BuildingNumber.One           },
            { "1.8" , BuildingNumber.OnePointEight },
            { "1.9" , BuildingNumber.OnePointNine  },
            { "1.10", BuildingNumber.OnePointTen   },
            { "2"   , BuildingNumber.Two           },
            { "2.1" , BuildingNumber.TwoPointOne   },
            { "2.2" , BuildingNumber.TwoPointTwo   },
            { "2.3" , BuildingNumber.TwoPointThree },
            { "2.4" , BuildingNumber.TwoPointFour  },
            { "2.5" , BuildingNumber.TwoPointFive  },
            { "2.6" , BuildingNumber.TwoPointSix   },
            { "2.7" , BuildingNumber.TwoPointSeven },
            { "3"   , BuildingNumber.Three         },
            { "4"   , BuildingNumber.Four          },
            { "5"   , BuildingNumber.Five          },
            { "6.1" , BuildingNumber.SixPointOne   },
            { "6.2" , BuildingNumber.SixPointTwo   },
            { "7.1" , BuildingNumber.SevenPointOne },
            { "7.2" , BuildingNumber.SevenPointTwo },
            { "8.1" , BuildingNumber.EightPointOne },
            { "8.2" , BuildingNumber.EightPointTwo },
            { "9"   , BuildingNumber.Nine          },
            { "10"  , BuildingNumber.Ten           },
            { "11"  , BuildingNumber.Eleven        },
            { "A"   , BuildingNumber.A             },
            { "B"   , BuildingNumber.B             },
            { "C"   , BuildingNumber.C             },
            { "D"   , BuildingNumber.D             },
            { "E"   , BuildingNumber.E             },
            { "F"   , BuildingNumber.F             },
            { "G"   , BuildingNumber.G             },
            { "L"   , BuildingNumber.L             },
            { "M"   , BuildingNumber.M             },
            { "S1"  , BuildingNumber.SOne          },
            { "S2"  , BuildingNumber.STwo          },
            { "S"   , BuildingNumber.S             },
            { "ул. Дер. 19", BuildingNumber.M19},
            { "ул. Дер. 19a", BuildingNumber.M19a},
            { "ул. Дер. 21", BuildingNumber.M21},
            { "ул. Державина, 19", BuildingNumber.M19},
            { "ул. Державина, 19a", BuildingNumber.M19a},
            { "ул. Державина, 21", BuildingNumber.M21},
            { "19", BuildingNumber.M19},
            { "19a", BuildingNumber.M19a },
            { "21", BuildingNumber.M21 }
        };

        public static BuildingNumber Convert(string name)
        {
            if (name.ToLower().Contains("все"))
                return BuildingNumber.All;

            var temp = name
                .ToUpper()
                .Replace(",", "")
                .Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var x in temp)
                if (bnDictionary.ContainsKey(x))
                    return bnDictionary[x];

            return BuildingNumber.Other;
        }
    }
}
