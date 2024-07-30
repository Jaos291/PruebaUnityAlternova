using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.IO;
using UnityEngine.TestTools;

public class GridTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void GridTestSimplePasses()
    {
        //Declaraciones
        var ObjetoPrueba = new GridManager();
        string path = Path.Combine(Application.streamingAssetsPath, "blocksTest.json");

        //Acciones
        ObjetoPrueba.UnitTesting();


        //Comprobaciones
        Assert.AreEqual(path, ObjetoPrueba.pathTest);
    }
}
