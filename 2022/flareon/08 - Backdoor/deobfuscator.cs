using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.IO;
using AsmResolver.PE.DotNet.Cil;
using System.Reflection;
using System.Data;
using System.Reflection.Emit;
using AsmResolver.PE.DotNet.Metadata.Tables;
using System.Runtime.InteropServices;
using AsmResolver.DotNet.Signatures.Types;
using System.Linq;
using AsmResolver.DotNet.Serialized;
using AsmResolver;
using AsmResolver.DotNet.Signatures;
using System.Security.Cryptography;
using System.Text;
using AsmResolver.PE.File;

using FlareOn.Backdoor;

namespace Resolver
{
    internal class Program
    {
        public const int IndentationWidth = 3;
        private const int Call = 0x28;
        static private String module_path;

        static private ModuleDefinition module;
        static private Module moduleReflected;
        static private Assembly assembly;
        static private AssemblyDefinition assemblyDefinition;
        static private Dictionary<string, Tuple<string, string, string>> obfuscatedFunctions;
        static private PEFile pe;

        static void Main(string[] args)
        {
            bool decryptFirstLayer = true;


            Dictionary<string, string> deobfuscatedIL = new Dictionary<string, string>();

            obfuscatedFunctions = new Dictionary<string, Tuple<string, string, string>>();
            obfuscatedFunctions.Add("flared_35", Tuple.Create<string, string, string>("FLARE09", "pe_b", "pe_m"));
            obfuscatedFunctions.Add("flared_47", Tuple.Create<string, string, string>("FLARE12", "d_b", "d_m"));
            obfuscatedFunctions.Add("flared_66", Tuple.Create<string, string, string>("FLARE15", "gh_b", "gh_m"));
            obfuscatedFunctions.Add("flared_67", Tuple.Create<string, string, string>("FLARE15", "cl_b", "cl_m"));
            obfuscatedFunctions.Add("flared_68", Tuple.Create<string, string, string>("FLARE15", "rt_b", ""));
            obfuscatedFunctions.Add("flared_69", Tuple.Create<string, string, string>("FLARE15", "gs_b", "gs_m"));
            obfuscatedFunctions.Add("flared_70", Tuple.Create<string, string, string>("FLARE15", "wl_b", "wl_m"));


            module_path = @"C:\Users\WIn10Base\Documents\flare\FlareOn.Backdoor.exe";
            pe = PEFile.FromFile(module_path);
            assembly = Assembly.LoadFile(module_path);

            var parameters = new ModuleReaderParameters();

            parameters.PEReaderParameters.ErrorListener = EmptyErrorListener.Instance;

            module = ModuleDefinition.FromFile(module_path, parameters);
            moduleReflected = assembly.GetModule("FlareOn.Backdoor.exe");

            CallInit(assembly);

            if (!decryptFirstLayer)
            {
                DecryptSecondLayer(obfuscatedFunctions);
                return;
            }
            // module = assemblyDefinition.Modules[0];
            DecryptSecondLayer(obfuscatedFunctions);
            /*     foreach (var type in module.TopLevelTypes)
                 {
                     foreach (var method in type.Methods)
                     {
                         //method.CilMethodBody.VerifyLabels(); continue;
                         if (obfuscatedFunctions.ContainsKey(method.Name)) continue;
                         try
                         {
                             var _ = method.CilMethodBody.Instructions.Size;
                             method.CilMethodBody.VerifyLabels();
                         }
                         catch
                         {
                             Console.WriteLine("Crashed at {0} cleaning method...\n", method.Name);
                             CilMethodBody methodBody = new CilMethodBody(method);
                             method.CilMethodBody = methodBody;
                         }
                     }
                 }*/


            foreach (KeyValuePair<string, Tuple<string, string, string>> keyValuePair in obfuscatedFunctions)
            {
                string type = "FlareOn.Backdoor.FLARE15";
                string methType = "FlareOn.Backdoor." + keyValuePair.Value.Item1;
                string methodName = keyValuePair.Key;

                string raw_body_field = keyValuePair.Value.Item2;
                string tokens_fields = keyValuePair.Value.Item3;

                byte[] raw_body = (byte[])assembly.GetType(type).GetField(raw_body_field).GetValue(null);
                Dictionary<uint, int> tokens = new Dictionary<uint, int>();
                if (tokens_fields.Length > 0)
                {
                    tokens = (Dictionary<uint, int>)assembly.GetType(type).GetField(tokens_fields).GetValue(null);
                }

                var methodInfo = moduleReflected.GetType(methType).GetMethod(methodName);
                string deobfuscated_IL = DeobfuscateFunction(assembly, methodName, methodInfo, raw_body, tokens);

                if (deobfuscated_IL == null)
                {
                    Console.WriteLine("Failed at {0}", methodName);
                }
                else
                {
                    string savePath = @"C:\Users\WIn10Base\Documents\flare\FlareOn.Backdoor.Deobfuscated." + methodName + ".txt";
                    File.WriteAllText(savePath, deobfuscated_IL);
                    Console.WriteLine("Saved {0} at {1}", methodName, savePath);
                }
            }



            module.Write(@"C:\Users\WIn10Base\Documents\flare\FlareOn.Backdoor.PartialDeobfuscated.exe");
        }

        private static string DeobfuscateFunction(Assembly assembly, string methodName, MethodInfo methodInfo, byte[] body, Dictionary<uint, int> tokens)
        {

            foreach (var type in module.TopLevelTypes)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Name.Value.Equals(methodName))
                    {
                        Console.WriteLine("{0}", method.Name);
                        Console.Write(method.MetadataToken);

                        return Deobfuscate(method, methodInfo, body, tokens);
                    }
                }
            }

            return null;
        }

        private static string Deobfuscate(MethodDefinition method, MethodInfo methodInfo, byte[] body, Dictionary<uint, int> tokens)
        {

            MethodBase methodBase = moduleReflected.ResolveMethod(methodInfo.MetadataToken);

            Type declaringType = methodBase.DeclaringType;

            var dynamicILInfo = new DynamicMethod("random", methodInfo.ReturnType, null, declaringType, true).GetDynamicILInfo();

            foreach (KeyValuePair<uint, int> keyValuePair in tokens)
            {
                int value = keyValuePair.Value;
                uint key = keyValuePair.Key;
                bool flag = value >= 1879048192 && value < 1879113727;
                int tokenFor;

                tokenFor = value;
                body[(int)key] = (byte)tokenFor;
                body[(int)(key + 1U)] = (byte)(tokenFor >> 8);
                body[(int)(key + 2U)] = (byte)(tokenFor >> 16);
                body[(int)(key + 3U)] = (byte)(tokenFor >> 24);
            }


            return WriteRawILToMethod(method, body, methodInfo);
        }


        private static string WriteRawILToMethod(MethodDefinition method, byte[] body, MethodInfo methodInfo)
        {
            var reader = ByteArrayDataSource.CreateReader(body);

            try
            {
                Console.WriteLine("There is {0} local variables", method.CilMethodBody.LocalVariables.Count);
            }
            catch
            {

                LocalVariablesSignature localVariables = null;
                if (module.TryLookupMember(methodInfo.GetMethodBody().LocalSignatureMetadataToken, out var member) &&
                member is StandAloneSignature { Signature: LocalVariablesSignature localVariablesSignature })
                {
                    localVariables = localVariablesSignature;
                }

                Console.WriteLine("Fixing variables from {0}", method.Name);
                CilMethodBody methodBody = new CilMethodBody(method);
                method.CilMethodBody = methodBody;

                if (localVariables != null)
                {
                    foreach (var variableType in localVariables.VariableTypes)
                    {
                        method.CilMethodBody.LocalVariables.Add(new CilLocalVariable(variableType));
                    }
                }

            }

            var cilDisassembler = new CilDisassembler(in reader,
                new PhysicalCilOperandResolver(method.Module, method.CilMethodBody));
            var instrs = cilDisassembler.ReadInstructions();

            Console.WriteLine("Cleaning...");
            method.CilMethodBody.ExceptionHandlers.Clear();
            method.CilMethodBody.Instructions.Clear();
            method.CilMethodBody.Instructions.AddRange(instrs);
           // method.CilMethodBody.ComputeMaxStackOnBuild = false;
            Console.WriteLine("{0} - {1}", method.Name, method.CilMethodBody.ExceptionHandlers.Count);

            string output = "";

            foreach (var instruction in method.CilMethodBody.Instructions)
            {
                output += instruction.ToString() + "\n";
            }


            return output;
        }
        private static void CallInit(Assembly assembly)
        {
            MethodInfo constructor = assembly.GetType("FlareOn.Backdoor.FLARE15").GetMethod("flare_74");
            constructor.Invoke(null, null);
        }



        private static void DecryptSecondLayer(Dictionary<string, Tuple<string, string, string>> excludeDictionary)
        {
            foreach (var type in module.TopLevelTypes)
            {
                foreach (var method in type.Methods)
                {
                    if (excludeDictionary.ContainsKey(method.Name)) continue;

                    if (method.Name.Value.Contains("flared_"))
                    {
                        string methodHash = GetMethodHash(method);
                        byte[] cilEnc = GetMethodData(methodHash);
                        if (cilEnc != null)
                        {
                            byte[] decryptedMethod = DecryptCIL(new byte[] { 18, 120, 171, 223 }, cilEnc);
                            byte[] fixedCil = FixMetadaTokens(decryptedMethod, method.MetadataToken.ToInt32(), null, method);
                            MethodInfo methodInfo = (MethodInfo)moduleReflected.ResolveMethod(method.MetadataToken.ToInt32());
                            WriteRawILToMethod(method, fixedCil, methodInfo);
                        }
                    }
                }

            }
        }

        private static byte[] GetMethodData(string methodHash)
        {
            foreach (var section in pe.Sections)
            {
                if (methodHash.StartsWith(section.Name))
                {
                    byte[] sectionData = new byte[section.GetVirtualSize()];
                    section.CreateReader().ReadBytes(sectionData, 0, (int)sectionData.Length); ;
                    return sectionData;
                }
            }
            return null;
        }

        private static byte[] DecryptCIL(byte[] p, byte[] d)
        {
            // RC4 encryption
            int[] array = new int[256];
            int[] array2 = new int[256];
            byte[] array3 = new byte[d.Length];
            int i;
            for (i = 0; i < 256; i++)
            {
                array[i] = (int)p[i % p.Length];
                array2[i] = i;
            }
            int num;
            for (i = (num = 0); i < 256; i++)
            {
                num = (num + array2[i] + array[i]) % 256;
                int num2 = array2[i];
                array2[i] = array2[num];
                array2[num] = num2;
            }
            int num3;
            num = (num3 = (i = 0));
            while (i < d.Length)
            {
                num3++;
                num3 %= 256;
                num += array2[num3];
                num %= 256;
                int num2 = array2[num3];
                array2[num3] = array2[num];
                array2[num] = num2;
                int num4 = array2[(array2[num3] + array2[num]) % 256];
                array3[i] = (byte)((int)d[i] ^ num4);
                i++;
            }
            return array3;
        }


        private static string GetMethodHash(MethodDefinition method)
        {

            string text = "";
            string text2 = "";
            MethodInfo methodInfo = (MethodInfo)moduleReflected.ResolveMethod(method.MetadataToken.ToInt32());
            MethodBody methodBody = methodInfo.GetMethodBody();
            byte[] bytes = Encoding.ASCII.GetBytes(methodInfo.Attributes.ToString());
            byte[] bytes2 = Encoding.ASCII.GetBytes(methodInfo.ReturnType.ToString());
            byte[] bytes3 = Encoding.ASCII.GetBytes(methodInfo.CallingConvention.ToString());
            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                string str = text2;
                Type parameterType = parameterInfo.ParameterType;
                text2 = str + ((parameterType != null) ? parameterType.ToString() : null);
            }
            byte[] bytes4 = Encoding.ASCII.GetBytes(methodBody.MaxStackSize.ToString());
            byte[] bytes5 = BitConverter.GetBytes(methodBody.GetILAsByteArray().Length);
            foreach (LocalVariableInfo localVariableInfo in methodBody.LocalVariables)
            {
                string str2 = text;
                Type localType = localVariableInfo.LocalType;
                text = str2 + ((localType != null) ? localType.ToString() : null);
            }
            byte[] bytes6 = Encoding.ASCII.GetBytes(text);
            byte[] bytes7 = Encoding.ASCII.GetBytes(text2);
            IncrementalHash incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            incrementalHash.AppendData(bytes5);
            incrementalHash.AppendData(bytes);
            incrementalHash.AppendData(bytes2);
            incrementalHash.AppendData(bytes4);
            incrementalHash.AppendData(bytes6);
            incrementalHash.AppendData(bytes7);
            incrementalHash.AppendData(bytes3);
            byte[] hashAndReset = incrementalHash.GetHashAndReset();
            StringBuilder stringBuilder = new StringBuilder(hashAndReset.Length * 2);
            for (int j = 0; j < hashAndReset.Length; j++)
            {
                stringBuilder.Append(hashAndReset[j].ToString("x2"));
            }
            return stringBuilder.ToString();
        }


        private static int GetNum(byte[] b, int o)
        {
            int num = (int)b[o + 3] * 16777216;
            num += (int)b[o + 2] * 65536;
            num += (int)b[o + 1] * 256;
            return num + (int)b[o];
        }

        public static byte[] FixMetadaTokens(byte[] b, int tk, object[] a, MethodDefinition methodDefinition)
        {
            Dictionary<uint, FLARE06.OT> dictionary = new Dictionary<uint, FLARE06.OT>
            {
                {
                    88U,
                    FLARE06.OT.A
                },
                {
                    214U,
                    FLARE06.OT.A
                },
                {
                    215U,
                    FLARE06.OT.A
                },
                {
                    95U,
                    FLARE06.OT.A
                },
                {
                    65024U,
                    FLARE06.OT.A
                },
                {
                    59U,
                    FLARE06.OT.D
                },
                {
                    46U,
                    FLARE06.OT.C
                },
                {
                    60U,
                    FLARE06.OT.D
                },
                {
                    47U,
                    FLARE06.OT.C
                },
                {
                    65U,
                    FLARE06.OT.D
                },
                {
                    52U,
                    FLARE06.OT.C
                },
                {
                    61U,
                    FLARE06.OT.D
                },
                {
                    48U,
                    FLARE06.OT.C
                },
                {
                    66U,
                    FLARE06.OT.D
                },
                {
                    53U,
                    FLARE06.OT.C
                },
                {
                    62U,
                    FLARE06.OT.D
                },
                {
                    49U,
                    FLARE06.OT.C
                },
                {
                    67U,
                    FLARE06.OT.D
                },
                {
                    54U,
                    FLARE06.OT.C
                },
                {
                    63U,
                    FLARE06.OT.D
                },
                {
                    50U,
                    FLARE06.OT.C
                },
                {
                    68U,
                    FLARE06.OT.D
                },
                {
                    55U,
                    FLARE06.OT.C
                },
                {
                    64U,
                    FLARE06.OT.D
                },
                {
                    51U,
                    FLARE06.OT.C
                },
                {
                    140U,
                    FLARE06.OT.B
                },
                {
                    56U,
                    FLARE06.OT.D
                },
                {
                    43U,
                    FLARE06.OT.C
                },
                {
                    1U,
                    FLARE06.OT.A
                },
                {
                    57U,
                    FLARE06.OT.D
                },
                {
                    44U,
                    FLARE06.OT.C
                },
                {
                    58U,
                    FLARE06.OT.D
                },
                {
                    45U,
                    FLARE06.OT.C
                },
                {
                    40U,
                    FLARE06.OT.B
                },
                {
                    41U,
                    FLARE06.OT.B
                },
                {
                    111U,
                    FLARE06.OT.B
                },
                {
                    116U,
                    FLARE06.OT.B
                },
                {
                    65025U,
                    FLARE06.OT.A
                },
                {
                    65026U,
                    FLARE06.OT.A
                },
                {
                    65027U,
                    FLARE06.OT.A
                },
                {
                    195U,
                    FLARE06.OT.A
                },
                {
                    65028U,
                    FLARE06.OT.A
                },
                {
                    65029U,
                    FLARE06.OT.A
                },
                {
                    65046U,
                    FLARE06.OT.B
                },
                {
                    211U,
                    FLARE06.OT.A
                },
                {
                    103U,
                    FLARE06.OT.A
                },
                {
                    104U,
                    FLARE06.OT.A
                },
                {
                    105U,
                    FLARE06.OT.A
                },
                {
                    106U,
                    FLARE06.OT.A
                },
                {
                    212U,
                    FLARE06.OT.A
                },
                {
                    138U,
                    FLARE06.OT.A
                },
                {
                    179U,
                    FLARE06.OT.A
                },
                {
                    130U,
                    FLARE06.OT.A
                },
                {
                    181U,
                    FLARE06.OT.A
                },
                {
                    131U,
                    FLARE06.OT.A
                },
                {
                    183U,
                    FLARE06.OT.A
                },
                {
                    132U,
                    FLARE06.OT.A
                },
                {
                    185U,
                    FLARE06.OT.A
                },
                {
                    133U,
                    FLARE06.OT.A
                },
                {
                    213U,
                    FLARE06.OT.A
                },
                {
                    139U,
                    FLARE06.OT.A
                },
                {
                    180U,
                    FLARE06.OT.A
                },
                {
                    134U,
                    FLARE06.OT.A
                },
                {
                    182U,
                    FLARE06.OT.A
                },
                {
                    135U,
                    FLARE06.OT.A
                },
                {
                    184U,
                    FLARE06.OT.A
                },
                {
                    136U,
                    FLARE06.OT.A
                },
                {
                    186U,
                    FLARE06.OT.A
                },
                {
                    137U,
                    FLARE06.OT.A
                },
                {
                    118U,
                    FLARE06.OT.A
                },
                {
                    107U,
                    FLARE06.OT.A
                },
                {
                    108U,
                    FLARE06.OT.A
                },
                {
                    224U,
                    FLARE06.OT.A
                },
                {
                    210U,
                    FLARE06.OT.A
                },
                {
                    209U,
                    FLARE06.OT.A
                },
                {
                    109U,
                    FLARE06.OT.A
                },
                {
                    110U,
                    FLARE06.OT.A
                },
                {
                    65047U,
                    FLARE06.OT.A
                },
                {
                    112U,
                    FLARE06.OT.B
                },
                {
                    91U,
                    FLARE06.OT.A
                },
                {
                    92U,
                    FLARE06.OT.A
                },
                {
                    37U,
                    FLARE06.OT.A
                },
                {
                    65041U,
                    FLARE06.OT.A
                },
                {
                    220U,
                    FLARE06.OT.A
                },
                {
                    65048U,
                    FLARE06.OT.A
                },
                {
                    65045U,
                    FLARE06.OT.B
                },
                {
                    117U,
                    FLARE06.OT.B
                },
                {
                    39U,
                    FLARE06.OT.B
                },
                {
                    65033U,
                    FLARE06.OT.F
                },
                {
                    2U,
                    FLARE06.OT.A
                },
                {
                    3U,
                    FLARE06.OT.A
                },
                {
                    4U,
                    FLARE06.OT.A
                },
                {
                    5U,
                    FLARE06.OT.A
                },
                {
                    14U,
                    FLARE06.OT.E
                },
                {
                    65034U,
                    FLARE06.OT.F
                },
                {
                    15U,
                    FLARE06.OT.E
                },
                {
                    32U,
                    FLARE06.OT.G
                },
                {
                    22U,
                    FLARE06.OT.A
                },
                {
                    23U,
                    FLARE06.OT.A
                },
                {
                    24U,
                    FLARE06.OT.A
                },
                {
                    25U,
                    FLARE06.OT.A
                },
                {
                    26U,
                    FLARE06.OT.A
                },
                {
                    27U,
                    FLARE06.OT.A
                },
                {
                    28U,
                    FLARE06.OT.A
                },
                {
                    29U,
                    FLARE06.OT.A
                },
                {
                    30U,
                    FLARE06.OT.A
                },
                {
                    21U,
                    FLARE06.OT.A
                },
                {
                    31U,
                    FLARE06.OT.E
                },
                {
                    33U,
                    FLARE06.OT.H
                },
                {
                    34U,
                    FLARE06.OT.G
                },
                {
                    35U,
                    FLARE06.OT.H
                },
                {
                    163U,
                    FLARE06.OT.B
                },
                {
                    151U,
                    FLARE06.OT.A
                },
                {
                    144U,
                    FLARE06.OT.A
                },
                {
                    146U,
                    FLARE06.OT.A
                },
                {
                    148U,
                    FLARE06.OT.A
                },
                {
                    150U,
                    FLARE06.OT.A
                },
                {
                    152U,
                    FLARE06.OT.A
                },
                {
                    153U,
                    FLARE06.OT.A
                },
                {
                    154U,
                    FLARE06.OT.A
                },
                {
                    145U,
                    FLARE06.OT.A
                },
                {
                    147U,
                    FLARE06.OT.A
                },
                {
                    149U,
                    FLARE06.OT.A
                },
                {
                    143U,
                    FLARE06.OT.B
                },
                {
                    123U,
                    FLARE06.OT.B
                },
                {
                    124U,
                    FLARE06.OT.B
                },
                {
                    65030U,
                    FLARE06.OT.B
                },
                {
                    77U,
                    FLARE06.OT.A
                },
                {
                    70U,
                    FLARE06.OT.A
                },
                {
                    72U,
                    FLARE06.OT.A
                },
                {
                    74U,
                    FLARE06.OT.A
                },
                {
                    76U,
                    FLARE06.OT.A
                },
                {
                    78U,
                    FLARE06.OT.A
                },
                {
                    79U,
                    FLARE06.OT.A
                },
                {
                    80U,
                    FLARE06.OT.A
                },
                {
                    71U,
                    FLARE06.OT.A
                },
                {
                    73U,
                    FLARE06.OT.A
                },
                {
                    75U,
                    FLARE06.OT.A
                },
                {
                    142U,
                    FLARE06.OT.A
                },
                {
                    65036U,
                    FLARE06.OT.F
                },
                {
                    6U,
                    FLARE06.OT.A
                },
                {
                    7U,
                    FLARE06.OT.A
                },
                {
                    8U,
                    FLARE06.OT.A
                },
                {
                    9U,
                    FLARE06.OT.A
                },
                {
                    17U,
                    FLARE06.OT.E
                },
                {
                    65037U,
                    FLARE06.OT.F
                },
                {
                    18U,
                    FLARE06.OT.E
                },
                {
                    20U,
                    FLARE06.OT.A
                },
                {
                    113U,
                    FLARE06.OT.B
                },
                {
                    126U,
                    FLARE06.OT.B
                },
                {
                    127U,
                    FLARE06.OT.B
                },
                {
                    114U,
                    FLARE06.OT.B
                },
                {
                    208U,
                    FLARE06.OT.B
                },
                {
                    65031U,
                    FLARE06.OT.B
                },
                {
                    221U,
                    FLARE06.OT.D
                },
                {
                    222U,
                    FLARE06.OT.C
                },
                {
                    65039U,
                    FLARE06.OT.A
                },
                {
                    198U,
                    FLARE06.OT.B
                },
                {
                    90U,
                    FLARE06.OT.A
                },
                {
                    216U,
                    FLARE06.OT.A
                },
                {
                    217U,
                    FLARE06.OT.A
                },
                {
                    101U,
                    FLARE06.OT.A
                },
                {
                    141U,
                    FLARE06.OT.B
                },
                {
                    115U,
                    FLARE06.OT.B
                },
                {
                    65049U,
                    FLARE06.OT.E
                },
                {
                    0U,
                    FLARE06.OT.A
                },
                {
                    102U,
                    FLARE06.OT.A
                },
                {
                    96U,
                    FLARE06.OT.A
                },
                {
                    38U,
                    FLARE06.OT.A
                },
                {
                    254U,
                    FLARE06.OT.A
                },
                {
                    253U,
                    FLARE06.OT.A
                },
                {
                    252U,
                    FLARE06.OT.A
                },
                {
                    251U,
                    FLARE06.OT.A
                },
                {
                    250U,
                    FLARE06.OT.A
                },
                {
                    249U,
                    FLARE06.OT.A
                },
                {
                    248U,
                    FLARE06.OT.A
                },
                {
                    255U,
                    FLARE06.OT.A
                },
                {
                    65054U,
                    FLARE06.OT.A
                },
                {
                    65053U,
                    FLARE06.OT.A
                },
                {
                    194U,
                    FLARE06.OT.B
                },
                {
                    93U,
                    FLARE06.OT.A
                },
                {
                    94U,
                    FLARE06.OT.A
                },
                {
                    42U,
                    FLARE06.OT.A
                },
                {
                    65050U,
                    FLARE06.OT.A
                },
                {
                    98U,
                    FLARE06.OT.A
                },
                {
                    99U,
                    FLARE06.OT.A
                },
                {
                    100U,
                    FLARE06.OT.A
                },
                {
                    65052U,
                    FLARE06.OT.B
                },
                {
                    65035U,
                    FLARE06.OT.F
                },
                {
                    16U,
                    FLARE06.OT.E
                },
                {
                    164U,
                    FLARE06.OT.B
                },
                {
                    155U,
                    FLARE06.OT.A
                },
                {
                    156U,
                    FLARE06.OT.A
                },
                {
                    157U,
                    FLARE06.OT.A
                },
                {
                    158U,
                    FLARE06.OT.A
                },
                {
                    159U,
                    FLARE06.OT.A
                },
                {
                    160U,
                    FLARE06.OT.A
                },
                {
                    161U,
                    FLARE06.OT.A
                },
                {
                    162U,
                    FLARE06.OT.A
                },
                {
                    125U,
                    FLARE06.OT.B
                },
                {
                    223U,
                    FLARE06.OT.A
                },
                {
                    82U,
                    FLARE06.OT.A
                },
                {
                    83U,
                    FLARE06.OT.A
                },
                {
                    84U,
                    FLARE06.OT.A
                },
                {
                    85U,
                    FLARE06.OT.A
                },
                {
                    86U,
                    FLARE06.OT.A
                },
                {
                    87U,
                    FLARE06.OT.A
                },
                {
                    81U,
                    FLARE06.OT.A
                },
                {
                    65038U,
                    FLARE06.OT.F
                },
                {
                    10U,
                    FLARE06.OT.A
                },
                {
                    11U,
                    FLARE06.OT.A
                },
                {
                    12U,
                    FLARE06.OT.A
                },
                {
                    13U,
                    FLARE06.OT.A
                },
                {
                    19U,
                    FLARE06.OT.E
                },
                {
                    129U,
                    FLARE06.OT.B
                },
                {
                    128U,
                    FLARE06.OT.B
                },
                {
                    89U,
                    FLARE06.OT.A
                },
                {
                    218U,
                    FLARE06.OT.A
                },
                {
                    219U,
                    FLARE06.OT.A
                },
                {
                    69U,
                    FLARE06.OT.I
                },
                {
                    65044U,
                    FLARE06.OT.A
                },
                {
                    122U,
                    FLARE06.OT.A
                },
                {
                    65042U,
                    FLARE06.OT.E
                },
                {
                    121U,
                    FLARE06.OT.B
                },
                {
                    165U,
                    FLARE06.OT.B
                },
                {
                    65043U,
                    FLARE06.OT.A
                },
                {
                    97U,
                    FLARE06.OT.A
                }
            };
            MethodBase methodBase = moduleReflected.ResolveMethod(tk);
            MethodInfo methodInfo = (MethodInfo)methodBase;
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] array = new Type[parameters.Length];
            SignatureHelper localVarSigHelper = SignatureHelper.GetLocalVarSigHelper();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = parameters[i].ParameterType;
            }
            Type declaringType = methodBase.DeclaringType;
            DynamicMethod dynamicMethod = new DynamicMethod("", methodInfo.ReturnType, array, declaringType, true);
            DynamicILInfo dynamicILInfo = dynamicMethod.GetDynamicILInfo();
            MethodBody methodBody = methodInfo.GetMethodBody();
            foreach (LocalVariableInfo localVariableInfo in methodBody.LocalVariables)
            {
                localVarSigHelper.AddArgument(localVariableInfo.LocalType);
            }
            byte[] signature = localVarSigHelper.GetSignature();
            dynamicILInfo.SetLocalSignature(signature);
            int j = 0;
            while (j < b.Length)
            {
                bool flag = b[j] == 254;
                uint key;
                if (flag)
                {
                    key = 65024U + (uint)b[j + 1];
                    j++;
                }
                else
                {
                    key = (uint)b[j];
                }
                FLARE06.OT ot = dictionary[key];
                j++;
                switch (ot)
                {
                    case FLARE06.OT.B:
                        {
                            uint num = (uint)GetNum(b, j);
                            num ^= 2727913149U;
                            bool flag2 = num >= 1879048192U && num < 1879113727U;
                            int tokenFor = (int)num;
                            b[j] = (byte)tokenFor;
                            b[j + 1] = (byte)(tokenFor >> 8);
                            b[j + 2] = (byte)(tokenFor >> 16);
                            b[j + 3] = (byte)(tokenFor >> 24);
                            j += 4;
                            break;
                        }
                    case FLARE06.OT.C:
                    case FLARE06.OT.E:
                        j++;
                        break;
                    case FLARE06.OT.D:
                    case FLARE06.OT.G:
                        j += 4;
                        break;
                    case FLARE06.OT.F:
                        j += 2;
                        break;
                    case FLARE06.OT.H:
                        j += 8;
                        break;
                    case FLARE06.OT.I:
                        j += 4 + GetNum(b, j) * 4;
                        break;
                }
            }

            return b;
        }
    }
}

