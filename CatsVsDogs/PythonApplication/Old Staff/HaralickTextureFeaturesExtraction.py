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



#negatives = glob('E:/Uczelnia/sem6/BIAI/train/cats/*.jpg')


positives = glob('E:/Uczelnia/sem6/BIAI/train/dogs/*.jpg')
with open('haraliccatsTest.csv', 'w') as f:
    writer = csv.writer(f, delimiter=',')           
    feature = features_for(positives[0])
    writer.writefield(
    writer.writerow([feature.tolist(), 0])


l = []
#unlabeled = glob('E:/Uczelnia/sem6/BIAI/test1/test1/*.jpg')
with open('haraliccatsTest.csv') as csvfile:
    reader = csv.DictReader(csvfile)
    for row in reader:
        l = row['array']
        print(row['array'], row['label'])


print(len(l))


