using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlareHR;


namespace FlareHR.Tests
{
    [TestClass]
    public class BattleShipTests
    {
        [TestMethod]
        public void CheckShipHasAlreadyBeenPlacedOnBoard()
        {
            var tracker = new BattleshipStateTracker();
            tracker.Init(ShipType.Cruiser, Alignment.Vertical, "B3");
            Assert.ThrowsException<System.Exception>(() => tracker.Init(ShipType.Cruiser, Alignment.Horizontal, "J9"));
        }

        [TestMethod]
        public void CheckShipPlacementClash()
        {
            var tracker = new BattleshipStateTracker();
            tracker.Init(ShipType.Carrier, Alignment.Vertical, "B3");
            Assert.ThrowsException<System.Exception>(() => tracker.Init(ShipType.Destroyer, Alignment.Horizontal, "A8"));
        }

        [TestMethod]
        public void CheckHitShot()
        {
            var tracker = new BattleshipStateTracker();
            tracker.Init(ShipType.Cruiser, Alignment.Vertical, "B3");
            Assert.IsTrue(tracker.RegisterShot("B4"), "Fired shot incorrectly deemed a miss");
        }

        [TestMethod]
        public void CheckMissShot()
        {
            var tracker = new BattleshipStateTracker();
            tracker.Init(ShipType.Cruiser, Alignment.Vertical, "B3");
            Assert.IsFalse(tracker.RegisterShot("B2"), "Fired shot incorrectly deemed a hit");
        }

        public void CheckInvalidCoordinateString()
        {
            var tracker = new BattleshipStateTracker();
            Assert.ThrowsException<System.Exception>(() => tracker.Init(ShipType.Cruiser, Alignment.Vertical, "B0"));
        }
    }
}
