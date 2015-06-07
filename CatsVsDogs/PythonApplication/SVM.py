from glob import glob
import mahotas
import mahotas.features
import milk
import numpy as np
import time
from jug import TaskGenerator
import csv

#@TaskGenerator
def learn_model(features, labels):
    learner = milk.defaultclassifier()
    return learner.train(features, labels)

#@TaskGenerator
def classify(model, features):
     return model.apply(features)

def features_from_file(filename):
    f = []
    with open(filename, "r") as file:
        for line in file:
            list = []
            features = line.split(",")
            for feature in features:
                list.append(float(feature))
            f.append(list)
        return f


def main():
    unlabeled = glob('E:/Uczelnia/sem6/BIAI/test1/test1/*.jpg')
    
    cats_features = features_from_file("cats_features.csv")
    dogs_features = features_from_file("dogs_features.csv")
    unlabeled_features = features_from_file("unlabeled_features.csv")

    f = cats_features + dogs_features
    labels = [0] * len(cats_features) + [1] * len(dogs_features)
   
    model = learn_model(f,labels)

    with open("output.csv", "w") as file:
        file.write("id, label\n")
        for i in range(len(unlabeled)):
            id = str(unlabeled[i]).split("\\")[1].split(".")[0]
            file.write(id +","+ str(classify(model, unlabeled_features[i])) + "\n")


if __name__ == '__main__':
    start_time = time.time();
    main()
    print("All done in %s seconds" % (time.time() - start_time))