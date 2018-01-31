using System;
using System.Collections.ObjectModel;
using System.IO;
using WiiScale.Logic.UI.BaseClasses;
using WiiScale.Logic.UI.Helper.FileSystem;
using WiiScale.Logic.UI.Helper.Serializer;

namespace WiiScale.Logic.UI.Model
{
    public class AccountSet : BaseModel
    {
        private string _accountPath = Path.Combine(AppDirectorySystemInfo.AppDataPath(AppSpecialFolder.Serializations),
            "Accounts.json");

        public AccountSet()
        {
            Init();
        }

        private void Init()
        {
            Accounts = new ObservableCollection<Account>();

            LoadAccounts();
        }

        private void LoadAccounts()
        {
            if (File.Exists(_accountPath))
                Accounts = JsonSerializer.DeserializeObject<ObservableCollection<Account>>(_accountPath);
        }

        public ObservableCollection<Account> Accounts { get; private set; }

        public override void Dispose()
        {
            base.Dispose();

            StoreAccounts();
        }

        private void StoreAccounts()
        {
            JsonSerializer.SerializeObject(Accounts, _accountPath);
        }
    }
}