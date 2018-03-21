// Copyright(c) 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.
using System.Collections.Generic;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.BigQuery
{
    public static class FipsIsoCountryMap
    {
        private static readonly Dictionary<string, string> Iso2Fips = new Dictionary<string, string>
        {
            // ISO3166-2, FIPS10-4
            { "AW", "AA" }, //          ARUBA
            { "AG", "AC" }, //          ANTIGUA AND BARBUDA
            { "AF", "AF" }, //          AFGHANISTAN
            { "DZ", "AG" }, //          ALGERIA
            { "AC", "AI" }, //          ASCENSION ISLAND
            { "AZ", "AJ" }, //          AZERBAIJAN
            { "AL", "AL" }, //          ALBANIA
            { "AM", "AM" }, //          ARMENIA
            { "AD", "AN" }, //          ANDORRA
            { "AO", "AO" }, //          ANGOLA
            { "AS", "AQ" }, //          AMERICAN SAMOA
            { "AR", "AR" }, //          ARGENTINA
            { "AU", "AS" }, //          AUSTRALIA
            //{ "", "AT" }, //          ASHMORE AND CARTIER ISLANDS
            { "AT", "AU" }, //          AUSTRIA
            { "AI", "AV" }, //          ANGUILLA
            { "KN", "AX" }, //          ANTIGUA, ST. KITTS, NEVIS, BARBUDA
            { "AQ", "AY" }, //          ANTARCTICA
            //{ "", "AZ" }, //          AZORES
            { "BH", "BA" }, //          BAHRAIN
            { "BB", "BB" }, //          BARBADOS
            { "BW", "BC" }, //          BOTSWANA
            { "BM", "BD" }, //          BERMUDA
            { "BE", "BE" }, //          BELGIUM
            { "BS", "BF" }, //          BAHAMAS THE
            { "BD", "BG" }, //          BANGLADESH
            { "BZ", "BH" }, //          BELIZE
            { "BA", "BK" }, //          BOSNIA AND HERZEGOVINA
            { "BO", "BL" }, //          BOLIVIA
            { "MM", "BM" }, //          BURMA
            { "BJ", "BN" }, //          BENIN
            { "BY", "BO" }, //          BELARUS
            { "SB", "BP" }, //          SOLOMON ISLANDS
            { "UM", "BQ" }, //          NAVASSA ISLAND
            { "BR", "BR" }, //          BRAZIL
            //{ "", "BS" }, //          BASSAS DA INDIA
            { "BT", "BT" }, //          BHUTAN
            { "BG", "BU" }, //          BULGARIA
            { "BV", "BV" }, //          BOUVET ISLAND
            { "BN", "BX" }, //          BRUNEI
            { "BI", "BY" }, //          BURUNDI
            //{ "BE", "BZ" }, //          BELGIUM AND LUXEMBOURG
            { "CA", "CA" }, //          CANADA
            { "KH", "CB" }, //          CAMBODIA
            { "EA", "CC" }, //          CEUTA AND MELILLA
            { "TD", "CD" }, //          CHAD
            { "LK", "CE" }, //          SRI LANKA
            { "CG", "CF" }, //          CONGO
            { "CD", "CG" }, //          ZAIRE
            { "CN", "CH" }, //          CHINA
            { "CL", "CI" }, //          CHILE
            { "KY", "CJ" }, //          CAYMAN ISLANDS
            { "CC", "CK" }, //          COCOS (KEELING) ISLANDS
            //{ "", "CL" }, //          CAROLINE ISLANDS
            { "CM", "CM" }, //          CAMEROON
            { "KM", "CN" }, //          COMOROS
            { "CO", "CO" }, //          COLOMBIA
            { "IC", "CP" }, //          CANARY ISLANDS
            { "MP", "CQ" }, //          NORTHERN MARIANA ISLANDS
            //{ "", "CR" }, //          CORAL SEA ISLANDS
            { "CR", "CS" }, //          COSTA RICA
            { "CF", "CT" }, //          CENTRAL AFRICAN REPUBLIC
            { "CU", "CU" }, //          CUBA
            { "CV", "CV" }, //          CAPE VERDE
            { "CK", "CW" }, //          COOK ISLANDS
            { "CY", "CY" }, //          CYPRUS
            { "CT", "CZ" }, //          CANTON ISLAND
            { "DK", "DA" }, //          DENMARK
            { "DJ", "DJ" }, //          DJIBOUTI
            { "DM", "DO" }, //          DOMINICA
            //{ "", "DQ" }, //          JARVIS ISLAND
            { "DO", "DR" }, //          DOMINICAN REPUBLIC
            { "YE", "DY" }, //          DEMOCRATIC YEMEN
            { "EC", "EC" }, //          ECUADOR
            { "EG", "EG" }, //          EGYPT
            { "IE", "EI" }, //          IRELAND
            { "GQ", "EK" }, //          EQUATORIAL GUINEA
            { "EE", "EN" }, //          ESTONIA
            { "ER", "ER" }, //          ERITREA
            { "SV", "ES" }, //          EL SALVADOR
            { "ET", "ET" }, //          ETHIOPIA
            //{ "", "EU" }, //          EUROPA ISLAND
            { "CZ", "EZ" }, //          CZECH REPUBLIC
            { "GF", "FG" }, //          FRENCH GUIANA
            { "FI", "FI" }, //          FINLAND
            { "FJ", "FJ" }, //          FIJI
            { "FK", "FK" }, //          FALKLAND ISLANDS (ISLAS MALVINAS)
            { "FM", "FM" }, //          MICRONESIA, FEDERATED STATES OF
            { "FO", "FO" }, //          FAROE ISLANDS
            { "PF", "FP" }, //          FRENCH POLYNESIA
            //{ "", "FQ" }, //          BAKER ISLAND
            { "FR", "FR" }, //          FRANCE
            { "TF", "FS" }, //          FRENCH SOUTHERN AND ANTARCTIC LANDS
            { "GM", "GA" }, //          GAMBIA  THE
            { "GA", "GB" }, //          GABON
            { "GE", "GG" }, //          GEORGIA
            { "GH", "GH" }, //          GHANA
            { "GI", "GI" }, //          GIBRALTAR
            { "GD", "GJ" }, //          GRENADA
            { "GG", "GK" }, //          GUERNSEY
            { "GL", "GL" }, //          GREENLAND
            { "DE", "GM" }, //          GERMANY
            //{ "", "GO" }, //          GLORIOSO ISLANDS
            { "GP", "GP" }, //          GUADELOUPE
            { "GU", "GQ" }, //          GUAM
            { "GR", "GR" }, //          GREECE
            { "GT", "GT" }, //          GUATEMALA
            { "GN", "GV" }, //          GUINEA
            { "GY", "GY" }, //          GUYANA
            { "PS", "GZ" }, //          GAZA STRIP
            { "HT", "HA" }, //          HAITI
            { "HK", "HK" }, //          HONG KONG
            { "HM", "HM" }, //          HEARD ISLAND AND MCDONALD ISLANDS
            { "HN", "HO" }, //          HONDURAS
            //{ "", "HQ" }, //          HOWLAND ISLAND
            { "HR", "HR" }, //          CROATIA
            { "HU", "HU" }, //          HUNGARY
            { "IS", "IC" }, //          ICELAND
            { "ID", "ID" }, //          INDONESIA
            { "IM", "IM" }, //          MAN  ISLE OF
            { "IN", "IN" }, //          INDIA
            { "IO", "IO" }, //          BRITISH INDIAN OCEAN TERRITORY
            { "CP", "IP" }, //          CLIPPERTON ISLAND
            { "IR", "IR" }, //          IRAN
            { "IL", "IS" }, //          ISRAEL
            { "IT", "IT" }, //          ITALY
            { "CI", "IV" }, //          COTE D'IVOIRE
            //{ "", "IW" }, //          ISRAEL-JORDAN DMZ
            { "IQ", "IZ" }, //          IRAQ
            { "JP", "JA" }, //          JAPAN
            { "JE", "JE" }, //          JERSEY
            { "JM", "JM" }, //          JAMAICA
            //{ "", "JN" }, //          JAN MAYEN
            { "JO", "JO" }, //          JORDAN
            //{ "", "JQ" }, //          JOHNSTON ATOLL
            //{ "", "JU" }, //          JUAN DE NOVA ISLAND
            { "KE", "KE" }, //          KENYA
            { "KG", "KG" }, //          KYRGYZSTAN
            { "KP", "KN" }, //          KOREA, NORTH
            //{ "", "KQ" }, //          KINGMAN REEF
            { "KI", "KR" }, //          KIRIBATI
            { "KR", "KS" }, //          KOREA, SOUTH
            { "CX", "KT" }, //          CHRISTMAS ISLAND
            { "KW", "KU" }, //          KUWAIT
            { "KZ", "KZ" }, //          KAZAKHSTAN
            { "LA", "LA" }, //          LAOS
            { "LC", "LC" }, //          ST. LUCIA AND ST. VINCENT
            { "LB", "LE" }, //          LEBANON
            { "LV", "LG" }, //          LATVIA
            { "LT", "LH" }, //          LITHUANIA
            { "LR", "LI" }, //          LIBERIA
            //{ "", "LN" }, //          SOUTHERN LINE ISLANDS
            { "SK", "LO" }, //          SLOVAKIA
            //{ "", "LQ" }, //          PALMYRA ATOLL
            { "LI", "LS" }, //          LIECHTENSTEIN
            { "LS", "LT" }, //          LESOTHO
            { "LU", "LU" }, //          LUXEMBOURG
            { "LY", "LY" }, //          LIBYA
            { "MG", "MA" }, //          MADAGASCAR
            { "MQ", "MB" }, //          MARTINIQUE
            { "MO", "MC" }, //          MACAU
            { "MD", "MD" }, //          MOLDOVA
            //{ "", "ME" }, //          MADEIRA
            { "YT", "MF" }, //          MAYOTTE
            { "MN", "MG" }, //          MONGOLIA
            { "MS", "MH" }, //          MONTSERRAT
            { "MW", "MI" }, //          MALAWI
            //{ "MK", "MK" }, //          MACEDONIA
            { "ML", "ML" }, //          MALI
            //{ "", "MM" }, //          BURMA (MYANMAR)
            { "MC", "MN" }, //          MONACO
            { "MA", "MO" }, //          MOROCCO
            { "MU", "MP" }, //          MAURITIUS
            //{ "", "MQ" }, //          MIDWAY ISLANDS
            { "MR", "MR" }, //          MAURITANIA
            { "MT", "MT" }, //          MALTA
            { "OM", "MU" }, //          OMAN
            { "MV", "MV" }, //          MALDIVES
            { "ME", "MW" }, //          MONTENEGRO
            { "MX", "MX" }, //          MEXICO
            { "MY", "MY" }, //          MALAYSIA
            { "MZ", "MZ" }, //          MOZAMBIQUE
            { "NC", "NC" }, //          NEW CALEDONIA
            { "NU", "NE" }, //          NIUE
            { "NF", "NF" }, //          NORFOLK ISLAND
            { "NE", "NG" }, //          NIGER
            { "VU", "NH" }, //          VANUATU
            { "NG", "NI" }, //          NIGERIA
            { "NL", "NL" }, //          NETHERLANDS
            { "NO", "NO" }, //          NORWAY
            { "NP", "NP" }, //          NEPAL
            { "NR", "NR" }, //          NAURU
            { "SR", "NS" }, //          SURINAME
            { "AN", "NT" }, //          NETHERLANDS ANTILLES
            { "NI", "NU" }, //          NICARAGUA
            { "NZ", "NZ" }, //          NEW ZEALAND
            //{ "", "OW" }, //          OCEAN WEATHER STATIONS
            { "PY", "PA" }, //          PARAGUAY
            { "PN", "PC" }, //          PITCAIRN ISLANDS
            { "PE", "PE" }, //          PERU
            //{ "", "PF" }, //          PARACEL ISLANDS
            //{ "", "PG" }, //          SPRATLY ISLANDS
            //{ "", "PI" }, //          PHOENIX ISLANDS
            { "PK", "PK" }, //          PAKISTAN
            { "PL", "PL" }, //          POLAND
            { "PA", "PM" }, //          PANAMA
            //{ "", "PN" }, //          NORTH PACIFIC ISLANDS
            { "PT", "PO" }, //          PORTUGAL
            { "PG", "PP" }, //          PAPUA NEW GUINEA
            { "PW", "PS" }, //          PALAU - TRUST TERRITORY OF THE PACIFIC ISLANDS
            { "GW", "PU" }, //          GUINEA-BISSAU
            //{ "", "PZ" }, //          SOUTH PACIFIC ISLANDS
            { "QA", "QA" }, //          QATAR
            { "RE", "RE" }, //          REUNION AND ASSOCIATED ISLANDS
            { "MH", "RM" }, //          MARSHALL ISLANDS
            { "RO", "RO" }, //          ROMANIA
            { "PH", "RP" }, //          PHILIPPINES
            { "PR", "RQ" }, //          PUERTO RICO
            { "RU", "RS" }, //          RUSSIA
            { "RW", "RW" }, //          RWANDA
            { "SA", "SA" }, //          SAUDI ARABIA
            //{ "", "SB" }, //          ST. PIERRE AND MIQUELON
            //{ "", "SC" }, //          ST. KITTS AND NEVIS
            { "SC", "SE" }, //          SEYCHELLES
            { "ZA", "SF" }, //          SOUTH AFRICA
            { "SN", "SG" }, //          SENEGAL
            { "SH", "SH" }, //          ST. HELENA
            { "SI", "SI" }, //          SLOVENIA
            //{ "", "SK" }, //          SARAWAK AND SABA
            { "SL", "SL" }, //          SIERRA LEONE
            { "SM", "SM" }, //          SAN MARINO
            { "SG", "SN" }, //          SINGAPORE
            { "SO", "SO" }, //          SOMALIA
            { "ES", "SP" }, //          SPAIN
            { "RS", "SR" }, //          SERBIA
            //{ "", "SS" }, //          ST. MAARTEN
            //{ "", "ST" }, //          ST. LUCIA
            { "SD", "SU" }, //          SUDAN
            { "SJ", "SV" }, //          SVALBARD
            { "SE", "SW" }, //          SWEDEN
            //{ "", "SX" }, //          SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS
            { "SY", "SY" }, //          SYRIA
            { "CH", "SZ" }, //          SWITZERLAND
            { "AE", "TC" }, //          UNITED ARAB EMIRATES
            { "TI", "TD" }, //          TRINIDAD AND TOBAGO
            //{ "", "TE" }, //          TROMELIN ISLAND
            { "TH", "TH" }, //          THAILAND
            { "TJ", "TI" }, //          TAJIKISTAN
            //{ "", "TK" }, //          TURKS AND CAICOS ISLANDS
            { "TK", "TL" }, //          TOKELAU
            { "TO", "TN" }, //          TONGA
            { "TG", "TO" }, //          TOGO
            { "ST", "TP" }, //          SAO TOME AND PRINCIPE
            { "TN", "TS" }, //          TUNISIA
            { "TR", "TU" }, //          TURKEY
            { "TV", "TV" }, //          TUVALU
            { "TW", "TW" }, //          TAIWAN
            { "TM", "TX" }, //          TURKMENISTAN
            { "TZ", "TZ" }, //          TANZANIA
            //{ "", "UA" }, //          FORMER USSR (ASIA)
            //{ "", "UE" }, //          FORMER USSR (EUROPE)
            { "UG", "UG" }, //          UGANDA
            { "GB", "UK" }, //          UNITED KINGDOM
            { "UA", "UP" }, //          UKRAINE
            { "US", "US" }, //          UNITED STATES
            { "BF", "UV" }, //          BURKINA FASO
            { "UY", "UY" }, //          URUGUAY
            { "UZ", "UZ" }, //          UZBEKISTAN
            { "VC", "VC" }, //          ST. VINCENT AND THE GRENADINES
            { "VE", "VE" }, //          VENEZUELA
            { "VG", "VI" }, //          VIRGIN ISLANDS (BRITISH)
            { "VN", "VM" }, //          VIETNAM
            { "VI", "VQ" }, //          VIRGIN ISLANDS (U.S.)
            { "VA", "VT" }, //          VATICAN CITY
            { "NA", "WA" }, //          NAMIBIA
            //{ "", "WE" }, //          WEST BANK
            { "WF", "WF" }, //          WALLIS AND FUTUNA
            { "EH", "WI" }, //          WESTERN SAHARA
            //{ "", "WQ" }, //          WAKE ISLAND
            //{ "WS", "WS" }, //          WESTERN SAMOA
            { "SZ", "WZ" }, //          SWAZILAND
            //{ "", "YM" }, //          YEMEN
            { "MK", "YU" }, //          YUGOSLAVIA (& FORMER TERRITORY)
            //{ "", "YY" }, //          ST. MARTEEN, ST. EUSTATIUS, AND SABA
            { "ZM", "ZA" }, //          ZAMBIA
            { "ZW", "ZI" }, //          ZIMBABWE
            { "WS", "ZM" }, //          SAMOA
            //{ "", "ZZ" }, //          ST. MARTIN AND ST. BARTHOLOMEW
        };

        public static string Map(string iso3166) => Iso2Fips.TryGetValue(iso3166, out string fips) ? fips : null;
    }
}
