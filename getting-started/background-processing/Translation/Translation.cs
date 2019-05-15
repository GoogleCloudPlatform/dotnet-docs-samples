using System;
using Google.Cloud.Firestore;

[FirestoreData]
public class Translation
{
    [FirestoreProperty]
    public string SourceText { get; set; }

    [FirestoreProperty]
    public string TranslatedText { get; set; }

    [FirestoreProperty]
    public DateTime TimeStamp { get; set; }
}
