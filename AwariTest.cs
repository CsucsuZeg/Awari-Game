using Microsoft.VisualStudio.TestTools.UnitTesting;
using AwariGameWpf.Model;
using AwariGameWpf.Persistence;
using Moq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace AwariTest
{
    [TestClass]
    public class TestAwari
    {

        private AwariModel model;
        private Mock<IDataAccess> mock;
        private Dictionary<int, int> mockedBowls;
        private Dictionary<int, int> mockedBowls2;
        private bool mockedActivePlayer;
        private bool mockedCanAgain;
        private int mockedSize;

        [TestInitialize]
        public void Initialize()
        {
            mockedBowls = new Dictionary<int, int>();
            mockedBowls2 = new Dictionary<int, int>();
            mockedActivePlayer = true;
            mockedCanAgain = false;
            mockedSize = 8;

            for (int i = 0; i < mockedSize + 2; i++)
            {
                if (i == mockedSize / 2 || i == mockedSize + 1)
                {
                    mockedBowls.Add(i, 0);
                }
                else
                {
                    mockedBowls.Add(i, 6);
                }
            }

            mock = new Mock<IDataAccess>();
            mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult((mockedBowls, mockedActivePlayer, mockedCanAgain)));

            model = new AwariModel(mock.Object);

        }

        [TestMethod]
        public void StartNewGameBowlsSize()
        {
            model.StartNewGame(mockedSize);

            Assert.AreEqual(mockedBowls.Count, model.bowls.Count);
        }

        [TestMethod]
        public void StartNewGameBowls()
        {
            model.StartNewGame(mockedSize);
            for (int i = 0; i < mockedBowls.Count; i++)
            {
                Assert.IsTrue(mockedBowls[i] == model.bowls[i]);
            }
        }

        [TestMethod]
        public void CheckGameOverIfRedWins()
        {
            model.StartNewGame(mockedSize);
            for (int i = 0; i < mockedSize + 2; i++)
            {
                if (i == mockedSize / 2)
                {
                    model.bowls[i] = 10;
                }
                else
                {
                    model.bowls[i] = 0;
                }
            }
            model.CheckGameOver();
            Assert.AreEqual("Red", model.winner);
        }

        [TestMethod]
        public void CheckGameOverIfBlueWins()
        {
            model.StartNewGame(mockedSize);
            for (int i = 0; i < mockedSize + 2; i++)
            {
                if (i == mockedSize + 1)
                {
                    model.bowls[i] = 10;
                }
                else
                {
                    model.bowls[i] = 0;
                }
            }
            model.CheckGameOver();
            Assert.AreEqual("Blue", model.winner);
        }

        [TestMethod]
        public void CheckGameOverIfTie()
        {
            model.StartNewGame(mockedSize);
            for (int i = 0; i < mockedSize + 2; i++)
            {
                if (i == mockedSize / 2 || i == mockedSize + 1)
                {
                    model.bowls[i] = 10;
                }
                else
                {
                    model.bowls[i] = 0;
                }
            }
            model.CheckGameOver();
            Assert.AreEqual("Tie", model.winner);
        }

        [TestMethod]
        public void CheckGameOverIfNoOneWins()
        {
            model.StartNewGame(mockedSize);
            for (int i = 0; i < mockedSize + 2; i++)
            {
                if (i == mockedSize / 2 || i == mockedSize + 1)
                {
                    model.bowls[i] = 10;
                }
                else
                {
                    model.bowls[i] = 1;
                }
            }
            model.CheckGameOver();
            Assert.AreEqual("", model.winner);
        }

        [TestMethod]
        public void RedActiveButRedCanAgain()
        {
            model.activePlayer = true;
            model.canAgain = true;
            mockedActivePlayer = model.WhoIsNext();
            Assert.IsTrue(mockedActivePlayer);
        }

        [TestMethod]
        public void RedActiveButRedCantAgain()
        {
            model.activePlayer = true;
            model.canAgain = false;
            mockedActivePlayer = model.WhoIsNext();
            Assert.IsFalse(mockedActivePlayer);
        }

        [TestMethod]
        public void NotRedActiveButNotRedCanAgain()
        {
            model.activePlayer = false;
            model.canAgain = true;
            mockedActivePlayer = model.WhoIsNext();
            Assert.IsFalse(mockedActivePlayer);
        }

        [TestMethod]
        public void NotRedActiveButNotRedCantAgain()
        {
            model.activePlayer = false;
            model.canAgain = false;
            mockedActivePlayer = model.WhoIsNext();
            Assert.IsTrue(mockedActivePlayer);
        }

        [TestMethod]
        public async Task AwariGameLoadTest()
        {
            model.StartNewGame(mockedSize);

            await model.LoadGame(String.Empty);

            for (int i = 0; i < model.bowls.Count; i++)
            {
                Assert.AreEqual(mockedBowls[i], model.bowls[i]);
            }

            Assert.IsTrue(model.activePlayer);
            Assert.IsFalse(model.canAgain);

            mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }
    }
}
