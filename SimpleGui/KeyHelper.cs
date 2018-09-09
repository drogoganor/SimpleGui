using System.Collections.Generic;
using Veldrid;

namespace SimpleGui
{
    public static class KeyHelper
    {
        private static readonly Dictionary<Key, char> NormalKeys = new Dictionary<Key, char>
            {
                    {Key.A, 'a'},
                    {Key.B, 'b'},
                    {Key.C, 'c'},
                    {Key.D, 'd'},
                    {Key.E, 'e'},
                    {Key.F, 'f'},
                    {Key.G, 'g'},
                    {Key.H, 'h'},
                    {Key.I, 'i'},
                    {Key.J, 'j'},
                    {Key.K, 'k'},
                    {Key.L, 'l'},
                    {Key.M, 'm'},
                    {Key.N, 'n'},
                    {Key.O, 'o'},
                    {Key.P, 'p'},
                    {Key.Q, 'q'},
                    {Key.R, 'r'},
                    {Key.S, 's'},
                    {Key.T, 't'},
                    {Key.U, 'u'},
                    {Key.V, 'v'},
                    {Key.W, 'w'},
                    {Key.X, 'x'},
                    {Key.Y, 'y'},
                    {Key.Z, 'z'},
                    {Key.Keypad0, '0'},
                    {Key.Keypad1, '1'},
                    {Key.Keypad2, '2'},
                    {Key.Keypad3, '3'},
                    {Key.Keypad4, '4'},
                    {Key.Keypad5, '5'},
                    {Key.Keypad6, '6'},
                    {Key.Keypad7, '7'},
                    {Key.Keypad8, '8'},
                    {Key.Keypad9, '9'},
                    {Key.Grave, '`'},
                    {Key.Minus, '-'},
                    {Key.Plus, '='},
                    {Key.BracketLeft, '['},
                    {Key.BracketRight, ']'},
                    {Key.BackSlash, '\\'},
                    {Key.Semicolon, ';'},
                    {Key.Quote, '\''},
                    {Key.Comma, ','},
                    {Key.Period, '.'},
                    {Key.Space, ' '}
                };

        private static readonly Dictionary<Key, char> ShiftedKeys = new Dictionary<Key, char>
            {
                    {Key.A, 'A'},
                    {Key.B, 'B'},
                    {Key.C, 'C'},
                    {Key.D, 'D'},
                    {Key.E, 'E'},
                    {Key.F, 'F'},
                    {Key.G, 'G'},
                    {Key.H, 'H'},
                    {Key.I, 'I'},
                    {Key.J, 'J'},
                    {Key.K, 'K'},
                    {Key.L, 'L'},
                    {Key.M, 'M'},
                    {Key.N, 'N'},
                    {Key.O, 'O'},
                    {Key.P, 'P'},
                    {Key.Q, 'Q'},
                    {Key.R, 'R'},
                    {Key.S, 'S'},
                    {Key.T, 'T'},
                    {Key.U, 'U'},
                    {Key.V, 'V'},
                    {Key.W, 'W'},
                    {Key.X, 'X'},
                    {Key.Y, 'Y'},
                    {Key.Z, 'Z'},
                    {Key.Keypad0, ')'},
                    {Key.Keypad1, '!'},
                    {Key.Keypad2, '@'},
                    {Key.Keypad3, '#'},
                    {Key.Keypad4, '$'},
                    {Key.Keypad5, '%'},
                    {Key.Keypad6, '^'},
                    {Key.Keypad7, '&'},
                    {Key.Keypad8, '*'},
                    {Key.Keypad9, '('},
                    {Key.Grave, '~'},
                    {Key.Minus, '_'},
                    {Key.Plus, '+'},
                    {Key.BracketLeft, '{'},
                    {Key.BracketRight, '}'},
                    {Key.BackSlash, '|'},
                    {Key.Semicolon, ':'},
                    {Key.Quote, '"'},
                    {Key.Comma, '<'},
                    {Key.Period, '>'},
                    {Key.Space, ' '},
                };


        public static bool IsInputKey(Key key, bool isShifted = false)
        {
            if (isShifted)
                return ShiftedKeys.ContainsKey(key);
            return NormalKeys.ContainsKey(key);
        }

        public static char GetKey(Key key, bool isShifted = false)
        {
            if (isShifted)
                return ShiftedKeys[key];
            return NormalKeys[key];
        }
    }
}
