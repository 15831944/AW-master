using System;
using System.Data;

using Layton.AuditWizard.DataAccess;
using NUnit.Framework;

namespace Layton.AuditWizard.Network.UnitTests
{
    [TestFixture]
    public class AddAssetTest
    {
        Asset _asset;
        private AssetDAO _assetDAO;

        [SetUp]
        public void Init()
        {
            _assetDAO = new AssetDAO();
            _asset = new Asset();

            _asset.Name = "test asset";
            _asset.AgentStatus = Asset.AGENTSTATUS.deployed;
            _asset.AgentVersion = "agent version 1";
            _asset.AlertsEnabled = false;
            _asset.AssetTag = "asset tag";
            _asset.AssetTypeID = 2;
            _asset.Auditable = true;
            _asset.Domain = "test";
            _asset.DomainID = 1;
            _asset.FullLocation = "full location";
            _asset.IPAddress = "1.2.3.4";
            _asset.LastAudit = DateTime.Now.AddDays(-1);
            _asset.Location = "location";
            _asset.LocationID = 1;
            _asset.MACAddress = "00:11:22:33";
            _asset.Make = "make";
            _asset.Model = "model";
            _asset.OverwriteData = true;
            _asset.ParentAssetID = 1;
            _asset.RequestAudit = false;
            _asset.SerialNumber = "AB12345";
            _asset.StockStatus = Asset.STOCKSTATUS.inuse;
            _asset.SupplierID = 1;
            _asset.SupplierName = "supplier";
            _asset.TypeAsString = "Computer";
            _asset.UniqueID = "ABCDE12345";
        }

        [Test]
        public void AddAsset()
        {
            int assetID = _assetDAO.AssetAdd(_asset);
            Assert.AreNotEqual(assetID, -1);
        }

        [Test]
        public void UpdateAsset()
        {
            _asset.Name = "updated";
            Assert.AreNotEqual(_asset.Name, "updated");

            _assetDAO.AssetUpdate(_asset);
            Assert.AreEqual(_asset.Name, "updated");
        }

        [TearDown]
        public void DeleteAsset()
        {
            _assetDAO.AssetDelete(_asset);
        }
    }
}
