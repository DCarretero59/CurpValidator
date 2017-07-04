using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CurpValidator
{
    #region enums

    /// <summary>
    /// Genres (enum)
    /// <para> Genres allowed for CURP Generation</para>
    /// </summary>
    public enum Genres
    {
        Male = 'H',
        Female = 'M'
    }

    #endregion

    #region Classes
    /// <summary>
    /// CurpClass (Class)
    /// <para> Class Library for the creation and/or verification of the first 16 digits of the Mexican CURP (Clave Única de Registro de Población/Unique Population Registry Code)
    /// </summary>
    public class CurpClass
    {
        #region Private Variables

        private static List<char> specialCharacters = new List<char>() { '/', '-', '.', '\\' };

        #endregion

        #region Public Variables
        #endregion

        #region Private Methods

        /// <summary>
        /// CreateLastNameCurpDigits(String lastName, out String curpDigit1_3, out char curpDigit14_15, bool isPaternalLastName = false)
        /// <para> Method to create CURP digits 1 through 3 and 14 and 15 from the given last name .</para>
        /// </summary>
        /// <param name = "lastName" type = "String">Given Last Name</param>
        /// <param name = "curpDigit1_3" type = " out String">Out Variable including CURP digit 1 and 2 if it's a paternal last name and 3 if it's mathernal last name</param>
        /// <param name = "curpDigit14_15" type = "out char">Out Variable including CURP digit 14 if it's a paternal last name and 15 if it's mathernal last name</param>
        /// <param name = "isPaternalLastName" type = "bool">Boolean that specifies if the given last name is paternal or not.</param>
        /// <returns> void </returns>
        private static void CreateLastNameCurpDigits(String lastName, out String curpDigit1_3, out char curpDigit14_15, bool isPaternalLastName = false)
        {
            try
            {
                bool consonantAdded = false;
                bool vowelAdded = false;
                bool isVowel;
                curpDigit1_3 = "";
                curpDigit14_15 = ' ';

                curpDigit1_3 += lastName[0] == 'Ñ' ? 'X' : specialCharacters.Contains(lastName[0]) ? 'X' : lastName[0];
                for (int i = 1; i < lastName.Length; i++)
                {
                    isVowel = IsLetterVowel(lastName[i]);
                    if (isVowel && !vowelAdded && isPaternalLastName)
                    {
                        curpDigit1_3 += lastName[i];
                        vowelAdded = true;
                    }
                    if (!isVowel && !consonantAdded)
                    {
                        curpDigit14_15 = lastName[i] == 'Ñ' ? 'X' : specialCharacters.Contains(lastName[i]) ? 'X' : lastName[i];
                        consonantAdded = true;
                        if (isPaternalLastName)
                        {
                            if (specialCharacters.Contains(lastName[i]) && !vowelAdded)
                            {
                                curpDigit1_3 += 'X';
                                vowelAdded = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (vowelAdded && consonantAdded && isPaternalLastName)
                    {
                        break;
                    }
                }
                if (!vowelAdded && isPaternalLastName)
                {
                    curpDigit1_3 += 'X';
                }
                if (!consonantAdded)
                {
                    curpDigit14_15 = 'X';
                }
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// CreateNameCurpDigits(String name, out char curpDigit4, out char curpDigit16)
        /// <para> Method for the creation of the 4th and 16th CURP digits from a given name.</para>
        /// </summary>
        /// <param name = "name" type = "String">Name</param>
        /// <param name = "curpDigit4" type = " out char">Out variable including CURP digit 4</param>
        /// <param name = "curpDigit16" type = "out char">Out variable including CURP digit 16</param>
        /// <returns> void </returns>
        private static void CreateNameCurpDigits(String name, out char curpDigit4, out char curpDigit16)
        {
            try
            {
                bool consonantAdded = false;
                bool isVowel;
                curpDigit16 = ' ';
                var compountName = name.Split(' ');
                string validatedName = name;
                if (compountName.Length > 0)
                {
                    if (compountName[0] == "MARIA" || compountName[0] == "MA." || compountName[0] == "MA" ||
                        compountName[0] == "JOSE" || compountName[0] == "J." || compountName[0] == "J")
                    {
                        validatedName = compountName[1];
                    }
                    else
                    {
                        validatedName = compountName[0];
                    }
                }
                curpDigit4 = validatedName[0] == 'Ñ' ? 'X' : specialCharacters.Contains(validatedName[0]) ? 'X' : validatedName[0];

                consonantAdded = false;
                for (int i = 1; i < validatedName.Length; i++)
                {
                    isVowel = IsLetterVowel(validatedName[i]);
                    if (!isVowel && !consonantAdded)
                    {
                        curpDigit16 = validatedName[i] == 'Ñ' ? 'X' : specialCharacters.Contains(validatedName[i]) ? 'X' : validatedName[i];
                        consonantAdded = true;
                        break;
                    }
                }
                if (!consonantAdded)
                {
                    curpDigit16 = 'X';
                }
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// FormatCurpText(String input)
        /// <para> Method to format the given text to meet the CURP generation criteria. </para>
        /// </summary>
        /// <param name="input" type = "String">Letra a Validar</param>
        /// <returns> String </returns>
        private static String FormatCurpText(String input)
        {

            try
            {
                List<String> prepositionConjunctionContraction = new List<String>() { "DA", "DAS", "DE", "DEL", "DER", "DI", "DIE", "DD",
                "EL", "LA", "LOS", "LAS", "LE", "LES", "MAC", "MC", "VAN", "VON", "Y" };

                input = RemoveDierecisAndAccents(input.ToUpper());
                while (input.Contains("  "))
                {
                    input = input.Replace("  ", " ");
                }

                var compountInput = input.Split(' ').ToList();
                if (compountInput.Count > 0)
                {
                    List<String> wordsToErase = new List<String>();
                    foreach (var compoundText in compountInput)
                    {
                        if (prepositionConjunctionContraction.Contains(compoundText))
                        {
                            wordsToErase.Add(compoundText);
                        }
                    }
                    foreach (var word in wordsToErase)
                    {
                        compountInput.Remove(word);
                    }
                    return String.Join(" ", compountInput);
                }
                else
                {
                    return input;
                }
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// IsLetterVowel(char letter)
        /// <para> Method to verify if the given letter is a vowel.</para>
        /// </summary>
        /// <param name = "letter" type = "char">Letra a Validar</param>
        /// <returns> bool </returns>
        private static bool IsLetterVowel(char letter)
        {
            try
            {
                return "aeiou".IndexOf(letter.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// RemoveDierecisAndAccents(String input)
        /// <para> Method to remove Diereseis and Accents from a given text.</para>
        /// </summary>
        /// <param name="input" type = "String">Letra a Validar</param>
        /// <returns> String </returns>
        private static String RemoveDierecisAndAccents(String input)
        {
            try
            {
                Regex a = new Regex("[Á|À|Ä|Â]", RegexOptions.Compiled);
                Regex e = new Regex("[É|È|Ë|Ê]", RegexOptions.Compiled);
                Regex i = new Regex("[Í|Ì|Ï|Î]", RegexOptions.Compiled);
                Regex o = new Regex("[Ó|Ò|Ö|Ô]", RegexOptions.Compiled);
                Regex u = new Regex("[Ú|Ù|Ü|Û]", RegexOptions.Compiled);
                input = a.Replace(input, "A");
                input = e.Replace(input, "E");
                input = i.Replace(input, "I");
                input = o.Replace(input, "O");
                input = u.Replace(input, "U");
                return input;
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// ValidateAndModifySonorousWords(String input)
        /// <para> Method to identify sonorous words according to the RENAPO regulation.</para>
        /// </summary>
        /// <param name="input" type = "String">Letra a Validar</param>
        /// <returns> String </returns>
        private static String ValidateAndModifySonorousWords(String input)
        {
            try
            {
                List<String> sonorousWords = new List<String>()
                {
                "BACA", "BAKA", "BUEI", "BUEY",
                "CACA", "CACO", "CAGA", "CAGO", "CAKA", "CAKO", "COGE", "COGI", "COJA", "COJE", "COJI", "COJO", "COLA", "CULO",
                "FALO", "FETO",
                "GETA", "GUEI", "GUEY",
                "JETA", "JOTO",
                "KACA", "KACO", "KAGA", "KAGO", "KAKA", "KAKO", "KOGE", "KOGI", "KOJA", "KOJE", "KOJI", "KOJO", "KOLA", "KULO",
                "LILO", "LOCA", "LOCO", "LOKA", "LOKO",
                "MAME", "MAMO", "MEAR", "MEAS", "MEON", "MIAR", "MION", "MOCO", "MOKO", "MULA", "MULO",
                "NACA", "NACO",
                "PEDA", "PEDO", "PENE", "PIPI", "PITO", "POPO", "PUTA", "PUTO",
                "QULO",
                "RATA", "ROBA", "ROBE", "ROBO", "RUIN",
                "SENO",
                "TETA",
                "VACA", "VAGA", "VAGO", "VAKA", "VUEI", "VUEY",
                "WUEI", "WUEY"
                };

                return sonorousWords.Contains(input) ? input.Replace(input[1], 'X') : input;
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// CreateCURP
        /// <para> CreateCURP(String name, String pathernalLastName, String mathernalLastName, DateTime dateOfBirth, Genres sex, FederalEntities federalEntity)
        /// </summary>
        /// <param name = "name" type = "String">Names</param>
        /// <param name = "pathernalLastName" type = "String">Pathernal Last Names</param>
        /// <param name = "mathernalLastName" type = "String">Mathernal Last Names</param>
        /// <param name = "dateOfBirth" type = "DateTime">Date of Birth</param>
        /// <param name = "sex" type = "Generos">Sex</param>
        /// <param name = "federalEntity" type = "FederalEntities">FederalEntity</param>
        /// <returns> String </returns>
        public static String CreateCURP(String name, String pathernalLastName, String mathernalLastName, DateTime dateOfBirth, Genres sex, FederalEntities federalEntity)
        {
            try
            {

                name = FormatCurpText(name.ToUpper());
                pathernalLastName = FormatCurpText(pathernalLastName.ToUpper());
                mathernalLastName = FormatCurpText(mathernalLastName.ToUpper());

                char curpDigit14 = new char();
                CreateLastNameCurpDigits(pathernalLastName, out String curpDigit1_2, out curpDigit14, true);

                String curpDigit3 = "";
                char curpDigit15 = new char();

                if (!string.IsNullOrEmpty(mathernalLastName))
                {
                    CreateLastNameCurpDigits(mathernalLastName, out curpDigit3, out curpDigit15);
                }
                else
                {
                    curpDigit3 = "X";
                    curpDigit15 = 'X';
                }

                char curpDigit4 = new char();
                char curpDigit16 = new char();
                CreateNameCurpDigits(name, out curpDigit4, out curpDigit16);

                String curpDigit1_4 = ValidateAndModifySonorousWords(curpDigit1_2 + curpDigit3 + curpDigit4);
                String curpDigit5_10 = dateOfBirth.ToString("yy") + dateOfBirth.ToString("MM") + dateOfBirth.ToString("dd");
                String curpDigit11 = Convert.ToChar(sex).ToString();
                String curpDigit12_13 = federalEntity.StateName;
                String curpDigit14_16 = curpDigit14.ToString() + curpDigit15 + curpDigit16;




                return curpDigit1_4 + curpDigit5_10 + curpDigit11 + curpDigit12_13 + curpDigit14_16;
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// FullNameMatchesCurp(String name, String pathernalLastName, String mathernalLastName, String curp)
        /// <para> Method to verify if the given person fullname matches the given curp.</para>
        /// </summary>
        /// <param name = "name" type = "String">Names</param>
        /// <param name = "pathernalLastName" type = "String">Pathernal Last Names</param>
        /// <param name = "mathernalLastName" type = "String">Mathernal Last Names</param>
        /// <param name = "curp" type = "String">CURP con 16 digitos a validar</param>
        /// <returns> bool </returns>
        public static bool FullNameMatchesCurp(String name, String pathernalLastName, String mathernalLastName, String curp)
        {
            try
            {
                String curpDigitCurp1_4 = curp.Substring(0, 4);
                String curpDigitCurp14_16 = curp.Substring(13, 3);
                String curpDigitValidation1_4 = "";
                String curpDigitValidation14_16 = "";

                name = FormatCurpText(name.ToUpper());
                pathernalLastName = FormatCurpText(pathernalLastName.ToUpper());
                mathernalLastName = FormatCurpText(mathernalLastName.ToUpper());

                char curpDigit14 = new char();
                CreateLastNameCurpDigits(pathernalLastName, out String curpDigit1_2, out curpDigit14, true);

                String curpDigit3 = "";
                char curpDigit15 = new char();

                if (!string.IsNullOrEmpty(mathernalLastName))
                {
                    CreateLastNameCurpDigits(mathernalLastName, out curpDigit3, out curpDigit15);
                }
                else
                {
                    curpDigit3 = "X";
                    curpDigit15 = 'X';
                }

                char curpDigit4 = new char();
                char curpDigit16 = new char();
                CreateNameCurpDigits(name, out curpDigit4, out curpDigit16);

                curpDigitValidation1_4 = curpDigit1_2 + curpDigit3 + curpDigit4;
                curpDigitValidation14_16 = curpDigit14.ToString() + curpDigit15 + curpDigit16;


                curpDigitValidation1_4 = ValidateAndModifySonorousWords(curpDigitValidation1_4);

                return curpDigitCurp1_4 == curpDigitValidation1_4 && curpDigitCurp14_16 == curpDigitValidation14_16;
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// ValidateCurp(String name, String pathernalLastName, String mathernalLastName, DateTime dateOfBirth, Genres sex, FederalEntities federalEntity, String curpToValidate)
        /// <para> Method for the validation of the first 16 digits of CURP using person fullname, date of birth, sex and it's federal entity o origin, against a given CURP.</para>
        /// </summary>
        /// <param name = "name" type = "String">Names</param>
        /// <param name = "pathernalLastName" type = "String">Pathernal Last Names</param>
        /// <param name = "mathernalLastName" type = "String">Mathernal Last Names</param>
        /// <param name = "dateOfBirth" type = "DateTime">Date of Birth</param>
        /// <param name = "sex" type = "Generos">Sex</param>
        /// <param name = "federalEntity" type = "FederalEntities">FederalEntity</param>
        /// <param name = "curpToValidate" type = "String">CURP input for validation</param>
        /// <returns> bool </returns>
        public static bool ValidateCurp(String name, String pathernalLastName, String mathernalLastName, DateTime dateOfBirth, Genres sex, FederalEntities federalEntity, String curpToValidate)
        {
            try
            {
                return CreateCURP(name, pathernalLastName, mathernalLastName, dateOfBirth, sex, federalEntity) == curpToValidate.Substring(0, 16);
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        #endregion




        

    } // public class ValidationCurp

    /// <summary>
    /// FederalEntities (Class)
    /// <para> Class to specify the Federal Entity that a person belongs from.</para>
    /// </summary>
    public class FederalEntities
    {
        private FederalEntities(String value) { StateName = value; }

        public String StateName { get; set; }

        public static FederalEntities Aguascalientes { get { return new FederalEntities("AS"); } }
        public static FederalEntities BajaCalifornia { get { return new FederalEntities("BC"); } }
        public static FederalEntities BajaCaliforniaSur { get { return new FederalEntities("BS"); } }
        public static FederalEntities Campeche { get { return new FederalEntities("CC"); } }
        public static FederalEntities Coahuila { get { return new FederalEntities("CL"); } }

        public static FederalEntities Colima { get { return new FederalEntities("CM"); } }
        public static FederalEntities Chiapas { get { return new FederalEntities("CS"); } }
        public static FederalEntities Chihuaha { get { return new FederalEntities("CH"); } }
        public static FederalEntities DistritoFederal { get { return new FederalEntities("DF"); } }
        public static FederalEntities Durango { get { return new FederalEntities("DG"); } }
        public static FederalEntities Guanajuato { get { return new FederalEntities("GT"); } }
        public static FederalEntities Guerrero { get { return new FederalEntities("GR"); } }
        public static FederalEntities Hidalgo { get { return new FederalEntities("HG"); } }
        public static FederalEntities Jalisco { get { return new FederalEntities("JC"); } }
        public static FederalEntities Mexico { get { return new FederalEntities("MC"); } }
        public static FederalEntities Michoacan { get { return new FederalEntities("MN"); } }
        public static FederalEntities Morelos { get { return new FederalEntities("MS"); } }
        public static FederalEntities Nayarit { get { return new FederalEntities("NT"); } }
        public static FederalEntities NuevoLeon { get { return new FederalEntities("NL"); } }
        public static FederalEntities Oaxaca { get { return new FederalEntities("OC"); } }
        public static FederalEntities Puebla { get { return new FederalEntities("PL"); } }
        public static FederalEntities Queretaro { get { return new FederalEntities("QT"); } }
        public static FederalEntities QuintanaRoo { get { return new FederalEntities("QR"); } }
        public static FederalEntities SanLuisPotosi { get { return new FederalEntities("SP"); } }
        public static FederalEntities Sinaloa { get { return new FederalEntities("SL"); } }
        public static FederalEntities Sonora { get { return new FederalEntities("SR"); } }
        public static FederalEntities Tabasco { get { return new FederalEntities("TC"); } }
        public static FederalEntities Tamaulipas { get { return new FederalEntities("TS"); } }
        public static FederalEntities Tlaxcala { get { return new FederalEntities("TL"); } }
        public static FederalEntities Veracruz { get { return new FederalEntities("VZ"); } }
        public static FederalEntities Yucatan { get { return new FederalEntities("YN"); } }
        public static FederalEntities Zacatecas { get { return new FederalEntities("ZS"); } }
        public static FederalEntities NacidoExtranjero { get { return new FederalEntities("NE"); } }
    }
    /// <summary>
    /// CurpGenerationErrorException (Class)
    /// <para> Custom Exception Class.</para>
    /// </summary>
    public class CurpGenerationErrorException : Exception
    {
        public CurpGenerationErrorException()
            : base() { }

        public CurpGenerationErrorException(string message)
            : base(message) { }

        public CurpGenerationErrorException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public CurpGenerationErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        public CurpGenerationErrorException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }
    }
    #endregion

} // namespace ValidadorCurp
