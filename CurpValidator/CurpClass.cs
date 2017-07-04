using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CurpValidator
{
    /// <summary>
    /// CurpClass (Class)
    /// <para> Clase que incluye funciones para validación y creación de los primeros 16 digitos del CURP
    /// </summary>
    public class CurpClass
    {

        private static List<char> specialCharacters = new List<char>() { '/', '-', '.', '\\' };

        /// <summary>
        /// CrearCURP
        /// <para> Método para crear los primeros 16 digitos del CURP de acuerdo al nombre, fecha de nacimiento, sexo y entidad federativa.</para>
        /// </summary>
        /// <param name = "name" type = "String">Nombre CURP</param>
        /// <param name = "pathernalLastName" type = "String">Apellido Paterno CURP</param>
        /// <param name = "mathernalLastName" type = "String">Apellido Materno CURP</param>
        /// <param name = "dateOfBirth" type = "DateTime">Fecha Nacimineto CURP</param>
        /// <param name = "sex" type = "Generos">Sexo CURP</param>
        /// <param name = "federalEntity" type = "EntidadesFederativas">Entidad Federativa CURP</param>
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
        /// CrearCURP
        /// <para> Método para verificar los primeros 16 digitos del CURP de acuerdo al nombre, fecha de nacimiento, sexo y entidad federativa.</para>
        /// </summary>
        /// <param name = "name" type = "String">Nombre CURP</param>
        /// <param name = "apellidoPaterno" type = "String">Apellido Paterno CURP</param>
        /// <param name = "apellidoMaterno" type = "String">Apellido Materno CURP</param>
        /// <param name = "fechaNacimiento" type = "DateTime">Fecha Nacimineto CURP</param>
        /// <param name = "sexo" type = "Generos">Sexo CURP</param>
        /// <param name = "entidadFederativa" type = "EntidadesFederativas">Entidad Federativa CURP</param>
        /// <param name = "curp" type = "String">CURP con 16 digitos a validar</param>
        /// <returns> bool </returns>
        public static bool ValidateCurp(String name, String pathernalLastName, String mathernalLastName, DateTime dateOfBirth, Genres sex, FederalEntities federalEntity, String curpToValidate)
        {
            try
            {
                return CreateCURP(name, pathernalLastName, mathernalLastName, dateOfBirth, sex, federalEntity) == curpToValidate;
            }
            catch (CurpGenerationErrorException e)
            {

                throw e;
            }
        }

        /// <summary>
        /// CoincideNombreCompletoConCurp
        /// <para> Método para verificar los digitos correspondientes al nombre, apellido paterno y apellido materno del CURP especificado.</para>
        /// </summary>
        /// <param name = "name" type = "String">Nombre a validar</param>
        /// <param name = "pathernalLastName" type = "String">Apellido Paterno  a validar</param>
        /// <param name = "mathernalLastName" type = "String">Apellido Materno  a validar</param>
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
        /// FormarParteApellidoCurp
        /// <para> Método para crear los digitos 1 a 3, y 14 y 15 del Curp a partir del apellido solicitado .</para>
        /// </summary>
        /// <param name = "lastName" type = "String">Apellido base</param>
        /// <param name = "curpDigit1_3" type = " out String">Variable de salida con la curpDigiticion 1 y 2 si es paterno, y 3 si es materno del CURP</param>
        /// <param name = "curpDigit14_15" type = "out char">Variable de salida con la curpDigiticion 14 si es paterno, y 15 si es materno del CURP</param>
        /// <param name = "isPaternalLastName" type = "bool">Booleano que especifica si el apellido dado es paterno.</param>
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
        /// FormarParteNombreCurp
        /// <para> Método para crear los digitos 4 y 16 del Curp a partir del nombre solicitado .</para>
        /// </summary>
        /// <param name = "name" type = "String">Apellido base</param>
        /// <param name = "curpDigit4" type = " out char">Variable de salida con la curpDigiticion 4 del CURP</param>
        /// <param name = "curpDigit16" type = "out char">Variable de salida con la curpDigiticion 16 del CURP</param>
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
        /// EsVocal
        /// <para> Método para verificar si la letra es vocal.</para>
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
        /// QuitarDierecisYAcentos
        /// <para> Método para eliminar dieresis y acentos del string dado.</para>
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
        /// FormatearTextoCurp
        /// <para> Método para identificar si el texto ingresado forma un nombre o apellido compuesto y remueve las precurpDigiticiones, conjunciones y contracciones de esta.</para>
        /// </summary>
        /// <param name="input" type = "String">Letra a Validar</param>
        /// <returns> String </returns>
        private static String FormatCurpText(String input)
        {

            try
            {
                List<String> precurpDigititionConjunctionContraction = new List<String>() { "DA", "DAS", "DE", "DEL", "DER", "DI", "DIE", "DD",
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
                        if (precurpDigititionConjunctionContraction.Contains(compoundText))
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
        /// ValidarYModificarAntisonante
        /// <para> Método para identificar si el texto dado es una palabra antisonante, y modificarla si esta lo es.</para>
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

    } // public class ValidationCurp

    /// <summary>
    /// Generos (enum)
    /// <para> Enumeración incluyendo los generos permitidos para la generación del CURP</para>
    /// </summary>
    public enum Genres
    {
        Male = 'H',
        Female = 'M'
    }
    /// <summary>
    /// EntidadesFederativas (Class)
    /// <para> Clase para especificar la Entidad Federativa a la cuál pertence la persona del CURP</para>
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
} // namespace ValidadorCurp
