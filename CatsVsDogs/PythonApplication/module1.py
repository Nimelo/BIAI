from glob import glob
import mahotas
import mahotas.features
import milk
import numpy as np
import time
from jug import TaskGenerator
import csv

#@TaskGenerator
def features_for(imname):
    img = mahotas.imread(imname)
    return mahotas.features.haralick(img).mean(0)

#@TaskGenerator
def learn_model(features, labels):
    learner = milk.defaultclassifier()
    return learner.train(features, labels)

#@TaskGenerator
def classify(model, features):
     return model.apply(features)


print("Start")
start_time = time.time();

print("Loading dogs's globs")

positives = glob('E:/Uczelnia/sem6/BIAI/train/dogs/*.jpg')

print("Loading cats's globs")
negatives = glob('E:/Uczelnia/sem6/BIAI/train/cats/*.jpg')

print("Loading unlabeled's globs")
unlabeled = glob('E:/Uczelnia/sem6/BIAI/test1/test1/*.jpg')

#positives = positives[:1000]
#negatives = negatives[:1000]
#unlabeled = unlabeled[]

f = []

iterator = 0
for i in negatives + positives:
    f.append(features_for(i))
    if iterator % 1000 == 0:
        print("Iteration:", iterator, time.time() - start_time) 
    iterator+=1

#features = map(features_for, negatives + positives)
labels = [0] * len(negatives) + [1] * len(positives)

print("Start learining")
learining_time = time.time()
model = learn_model(f, labels)
print("Learned in:", time.time() - learining_time)
  
#labeled = [classify(model, features_for(u)) for u in unlabeled[:1]]

#feature = features_for(unlabeled[0])
#label = classify(model, feature)

cats = 0
dogs = 0
idk = 0

print("Labeling")
iterator = 0
lab_time = time.time()
with open('svnMahotasFUll.csv', 'w') as f:
    writer = csv.writer(f, delimiter=',')   
    for i in unlabeled:
        if iterator % 1000 == 0:
            print("Iteration:", iterator, time.time() - lab_time)
        iterator += 1
        feature = features_for(i)
        label = classify(model, feature)
        writer.writerow([i, label])
        if label == 0:
            cats+=1
        elif label == 1:
            dogs+=1
        else:
            idk+=1
print(cats)
print(dogs)
print(idk)

print("All in", time.time() - start_time)

