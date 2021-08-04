using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class TreasureParams {
    public string type;
    public string cr;

    public TreasureParams() {
        type = "individual";
        cr = "Challenge 0-4";
    }
}

public class TreasureDataManager : Singleton<TreasureDataManager> {

    private Treasure treasure;
    private List<MagicItem> magicItems;
    private List<Book> books;
    private TreasureParams treasureParam = new TreasureParams();

    public void LoadData() {
        if (this.treasure == null) {
            this.treasure = JsonConvert.DeserializeObject<Treasure>(Json.LoadJson("treasureLoot.json"));
        }
        if (this.magicItems == null) {
            this.magicItems = JsonConvert.DeserializeObject<List<MagicItem>>(Json.LoadJson("magicItems.json"));
        }
        if (this.books == null) {
            Books oBooks = JsonConvert.DeserializeObject<Books>(Json.LoadJson("books.json"));
            this.books = oBooks.book;
        }
    }

    public void Save() {
        JsonSerializer serializer = new JsonSerializer();
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/savedata.gd")) {
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                serializer.Serialize(writer, this.treasureParam);
            }
        }
    }

    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/savedata.gd")) {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/savedata.gd")) {
                using (JsonReader reader = new JsonTextReader(sr)) {
                    this.treasureParam = serializer.Deserialize<TreasureParams>(reader);
                }
            }
        }
    }

    public Treasure Treasure {
        get { return this.treasure; }
	}

    public List<MagicItem> MagicItems {
        get { return this.magicItems; }
	}

    public List<Book> Books {
        get { return this.books; }
    }

    public TreasureParams TreasureParams {
        get { return this.treasureParam; }
        set { this.treasureParam = value; }
	}
}