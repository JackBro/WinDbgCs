﻿using CsDebugScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbgEngTest
{
    /// <summary>
    /// E2E tests for verifying various functionalities of CsScript against NativeDumpTest.VS2013.exe.
    /// </summary>
    [TestClass]
    public class NativeDumpTest32GCC : TestBase
    {
        private const string DefaultDumpFile = @"..\..\..\dumps\NativeDumpTest.gcc.mdmp";
        private const string DefaultModuleName = "NativeDumpTest_gcc";
        private const string DefaultSymbolPath = @"..\..\..\dumps\";

        private static NativeDumpTest testRunner;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            SyncStart();
            testRunner = new NativeDumpTest(DefaultDumpFile, DefaultModuleName, DefaultSymbolPath);
            testRunner.TestSetup();
        }

        [ClassCleanup]
        public static void TestCleanup()
        {
            SyncStop();
        }

        [TestMethod]
        public void TestModuleExtraction()
        {
            testRunner.TestModuleExtraction();
        }

        [TestMethod]
        public void ReadingFloatPointTypes()
        {
            testRunner.ReadingFloatPointTypes();
        }

        [TestMethod]
        public void GettingClassStaticMember()
        {
            // TODO: cv2pdb doesn't export static members
            // testRunner.GettingClassStaticMember();
        }

        [TestMethod]
        public void CheckProcess()
        {
            testRunner.CheckProcess();
        }

        [TestMethod]
        public void CheckDebugger()
        {
            testRunner.CheckDebugger();
        }

        [TestMethod]
        public void CurrentThreadContainsNativeDumpTestCpp()
        {
            testRunner.CurrentThreadContainsNativeDumpTestCpp();
        }

        [TestMethod]
        public void CurrentThreadContainsNativeDumpTestMainFunction()
        {
            testRunner.CurrentThreadContainsNativeDumpTestMainFunction();
        }

        [TestMethod]
        public void CheckMainArguments()
        {
            // TODO: cv2pdb exports arguments as local variable
            //testRunner.CheckMainArguments();
        }

        [TestMethod]
        public void CheckThread()
        {
            testRunner.CheckThread();
        }

        [TestMethod]
        public void CheckCodeArray()
        {
            testRunner.CheckCodeArray();
        }

        [TestMethod]
        public void CheckCodeFunction()
        {
            testRunner.CheckCodeFunction();
        }

        [TestMethod]
        public void CheckMainLocals()
        {
            testRunner.CheckDefaultTestCaseLocals();
        }
    }
}
