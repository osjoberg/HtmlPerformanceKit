namespace HtmlSpeedPack.Infrastructure
{
    internal class HtmlChar
    {
        internal const char Null = '\0';
        internal const char ReplacementCharacter = '\uFFFD';

        internal static int ParseDecimalDigit(int @char)
        {
            return @char >= '0' && @char <= '9' ? @char - '0' : -1;
        }

        internal static int ParseHexDigit(int @char)
        {
            if (@char >= '0' && @char <= '9')
            {
                return @char - '0';
            }

            if (@char >= 'a' && @char <= 'f')
            {
                return @char - 'a' + 10;
            }

            if (@char >= 'A' && @char <= 'F')
            {
                return @char - 'A' + 10;
            }

            return -1;
        }

        internal static char? GetReplacementCharacterReference(int codepoint)
        {
            switch (codepoint)
            {
                case 0x00: return '\xFFFD'; // REPLACEMENT CHARACTER
                case 0x80: return '\x20AC'; // EURO SIGN (€)
                case 0x82: return '\x201A'; // SINGLE LOW-9 QUOTATION MARK (‚)
                case 0x83: return '\x0192'; // LATIN SMALL LETTER F WITH HOOK (ƒ)
                case 0x84: return '\x201E'; // DOUBLE LOW-9 QUOTATION MARK („)
                case 0x85: return '\x2026'; // HORIZONTAL ELLIPSIS (…)
                case 0x86: return '\x2020'; // DAGGER (†)
                case 0x87: return '\x2021'; // DOUBLE DAGGER (‡)
                case 0x88: return '\x02C6'; // MODIFIER LETTER CIRCUMFLEX ACCENT (ˆ)
                case 0x89: return '\x2030'; // PER MILLE SIGN (‰)
                case 0x8A: return '\x0160'; // LATIN CAPITAL LETTER S WITH CARON (Š)
                case 0x8B: return '\x2039'; // SINGLE LEFT-POINTING ANGLE QUOTATION MARK (‹)
                case 0x8C: return '\x0152'; // LATIN CAPITAL LIGATURE OE (Œ)
                case 0x8E: return '\x017D'; // LATIN CAPITAL LETTER Z WITH CARON (Ž)
                case 0x91: return '\x2018'; // LEFT SINGLE QUOTATION MARK (‘)
                case 0x92: return '\x2019'; // RIGHT SINGLE QUOTATION MARK (’)
                case 0x93: return '\x201C'; // LEFT DOUBLE QUOTATION MARK (“)
                case 0x94: return '\x201D'; // RIGHT DOUBLE QUOTATION MARK (”)
                case 0x95: return '\x2022'; // BULLET (•)
                case 0x96: return '\x2013'; // EN DASH (–)
                case 0x97: return '\x2014'; // EM DASH (—)
                case 0x98: return '\x02DC'; // SMALL TILDE (˜)
                case 0x99: return '\x2122'; // TRADE MARK SIGN (™)
                case 0x9A: return '\x0161'; // LATIN SMALL LETTER S WITH CARON (š)
                case 0x9B: return '\x203A'; // SINGLE RIGHT-POINTING ANGLE QUOTATION MARK (›)
                case 0x9C: return '\x0153'; // LATIN SMALL LIGATURE OE (œ)
                case 0x9E: return '\x017E'; // LATIN SMALL LETTER Z WITH CARON (ž)
                case 0x9F: return '\x0178'; // LATIN CAPITAL LETTER Y WITH DIAERESIS (Ÿ)
                default: return null;
            }
        }

        internal static bool IsCharacterReferenceReplacementToken(int codepoint)
        {
            return (codepoint >= 0xD800 && codepoint <= 0xDFFF) || codepoint >= 0x10FFFF;
        }

        internal static bool IsCharacterReferenceParseError(int codepoint)
        {
            if ((codepoint >= 0x0001 && codepoint <= 0x0008) ||
                (codepoint >= 0x000D && codepoint <= 0x001F) ||
                (codepoint >= 0x007F && codepoint <= 0x009F) ||
                (codepoint >= 0xFDD0 && codepoint <= 0xFDEF))
            {
                return true;
            }

            switch (codepoint)
            {
                case 0x000B: 
                case 0xFFFE:
                case 0xFFFF: 
                case 0x1FFFE: 
                case 0x1FFFF: 
                case 0x2FFFE: 
                case 0x2FFFF: 
                case 0x3FFFE: 
                case 0x3FFFF: 
                case 0x4FFFE: 
                case 0x4FFFF: 
                case 0x5FFFE: 
                case 0x5FFFF: 
                case 0x6FFFE: 
                case 0x6FFFF: 
                case 0x7FFFE: 
                case 0x7FFFF: 
                case 0x8FFFE: 
                case 0x8FFFF: 
                case 0x9FFFE: 
                case 0x9FFFF: 
                case 0xAFFFE: 
                case 0xAFFFF: 
                case 0xBFFFE: 
                case 0xBFFFF: 
                case 0xCFFFE: 
                case 0xCFFFF: 
                case 0xDFFFE: 
                case 0xDFFFF: 
                case 0xEFFFE: 
                case 0xEFFFF: 
                case 0xFFFFE:
                case 0xFFFFF: 
                case 0x10FFFE:
                case 0x10FFFF:
                    return true;

                default:
                    return false;
            }
        }
    }
}
