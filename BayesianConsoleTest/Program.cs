// See https://aka.ms/new-console-template for more information
using BayesianClassifier;


NaiveBayes<string> nb = new(0.3, 0.9);
nb.Train("Roger", "man");
nb.Train("Roger", "code");
nb.Train("Roger", "man");
nb.Train("Roger", "man");
nb.Train("Roger", "man");
nb.Train("Roger", "man");
nb.Train("Cheese", "food");
nb.Train("Sandwich", "food");
nb.Train("Sandwich", "food");
nb.Train("Cheese", "food");
nb.Train("Cheese", "dairy product");
nb.Train("Cheese", "dairy product");

List<string> features1 = new();
features1.Add("Roger");

Console.Write("\nRoger is classified as ");
Console.Write(nb.Classify(features1));

List<string> features2 = new();
features2.Add("Cheese");
features2.Add("Sandwich");

Console.Write("\n(Cheese, Sandwich) is classified as ");
Console.Write(nb.Classify(features2));

Console.Write("\n-----------------------------------");
nb.SaveClassifier("nb");

NaiveBayes<int> nb1 = new(0.3, 0.9);
nb1.Train(1111, "man");
nb1.Train(1111, "code");
nb1.Train(1111, "man");
nb1.Train(1111, "man");
nb1.Train(1111, "man");
nb1.Train(1111, "man");
nb1.Train(2222, "food");
nb1.Train(2222, "dairy product");
nb1.Train(2222, "dairy product");
nb1.Train(3333, "food");
nb1.Train(3333, "food");
nb1.Train(3333, "food");

List<int> features3 = new();
features3.Add(1111);

Console.Write("\n1111 is classified as ");
Console.Write(nb1.Classify(features3));

List<int> features4 = new();
features4.Add(2222);
features4.Add(3333);

Console.Write("\n(2222, 3333) is classified as ");
Console.Write(nb1.Classify(features4));

Console.ReadKey(); Console.Write("\n-----------------------------------");

Console.WriteLine("\nLoading stored classifier nb");
NaiveBayes<string> nb_copy = new NaiveBayes<string>(0.3, 0.9);
nb_copy.LoadClassifier("nb");

Console.Write("\n(Cheese, Sandwich) is classified by the loaded classifier as ");
Console.Write(nb_copy.Classify(features2));

Console.ReadKey();
