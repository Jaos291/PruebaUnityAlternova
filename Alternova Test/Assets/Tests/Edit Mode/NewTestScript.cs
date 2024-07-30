using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestSetupBlock()
    {
        //Declaraciones
        var ObjetoPrueba = new BlockBehaviour();
        var num = 0;

        //Acciones
        ObjetoPrueba.UnitTestForSetup();

        //Comprobaciones
        Assert.AreEqual(num, ObjetoPrueba.number);
    }

}
