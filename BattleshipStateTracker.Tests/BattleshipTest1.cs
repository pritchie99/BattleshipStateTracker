using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlareHR.Tests
{
    [TestClass]
    public class BattleShipTests
    {
        [TestMethod]
        public void CheckShipHasAlreadyBeenPlacedOnBoard()
        {
            var tracker = new BattleshipStateTracker();
            tracker.AddShip(ShipType.Cruiser, Alignment.Vertical, "B3");
            Assert.ThrowsException<System.Exception>(() => tracker.AddShip(ShipType.Cruiser, Alignment.Horizontal, "J9"));
        }

        [TestMethod]
        public void CheckShipPlacementClash()
        {
            var tracker = new BattleshipStateTracker();
            tracker.AddShip(ShipType.Carrier, Alignment.Vertical, "B3");
            Assert.ThrowsException<System.Exception>(() => tracker.AddShip(ShipType.Destroyer, Alignment.Horizontal, "A7"));
        }

        [TestMethod]
        public void CheckHitShots()
        {
            var tracker = new BattleshipStateTracker();
            tracker.AddShip(ShipType.Cruiser, Alignment.Vertical, "B3");
            Assert.IsTrue(tracker.RegisterShot("B3"), "Fired shot B3 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("B4"), "Fired shot B4 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("B5"), "Fired shot B5 incorrectly deemed a miss");

        }

        [TestMethod]
        public void CheckMissShots()
        {
            var tracker = new BattleshipStateTracker();
            tracker.AddShip(ShipType.Cruiser, Alignment.Vertical, "B3");
            Assert.IsFalse(tracker.RegisterShot("B2"), "Fired shot B2 incorrectly deemed a hit");
            Assert.IsFalse(tracker.RegisterShot("B6"), "Fired shot B6 incorrectly deemed a hit");
            Assert.IsFalse(tracker.RegisterShot("A3"), "Fired shot A3 incorrectly deemed a hit");
            Assert.IsFalse(tracker.RegisterShot("C3"), "Fired shot C3 incorrectly deemed a hit");
        }

        [TestMethod]
        public void CheckInvalidCoordinateString()
        {
            var tracker = new BattleshipStateTracker();
            Assert.ThrowsException<System.Exception>(() => tracker.AddShip(ShipType.Cruiser, Alignment.Horizontal, "B11"));
            Assert.ThrowsException<System.Exception>(() => tracker.AddShip(ShipType.Cruiser, Alignment.Horizontal, ""));
            Assert.ThrowsException<System.Exception>(() => tracker.AddShip(ShipType.Cruiser, Alignment.Horizontal, "A-1"));
            Assert.ThrowsException<System.Exception>(() => tracker.AddShip(ShipType.Cruiser, Alignment.Horizontal, "ABC"));
            Assert.ThrowsException<System.Exception>(() => tracker.AddShip(ShipType.Cruiser, Alignment.Horizontal, "11"));
        }

        [TestMethod]
        public void CheckAllShipsSunk()
        {
            var tracker = new BattleshipStateTracker();
            tracker.AddShip(ShipType.Cruiser, Alignment.Vertical, "B3");
            tracker.AddShip(ShipType.Carrier, Alignment.Horizontal, "E8");

            Assert.IsTrue(tracker.RegisterShot("B3"), "Fired shot B3 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("B4"), "Fired shot B4 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("B5"), "Fired shot B5 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("E8"), "Fired shot E8 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("F8"), "Fired shot F8 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("G8"), "Fired shot G8 incorrectly deemed a miss");
            Assert.IsTrue(tracker.RegisterShot("H8"), "Fired shot H8 incorrectly deemed a miss");

            Assert.IsFalse(tracker.AllShipsSunk(), "All Ships Sunk incorrectly deemed true before last shot taken");

            Assert.IsTrue(tracker.RegisterShot("I8"), "Fired shot I8 incorrectly deemed a miss");

            Assert.IsTrue(tracker.AllShipsSunk(), "All Ships Sunk incorrectly deemed false");
        }



    }
}
