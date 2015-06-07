from glob import glob
import mahotas
import mahotas.features
import milk
import numpy as np
import time
from jug import TaskGenerator
import csv
import time

#@TaskGenerator
def features_for(imname):
    img = mahotas.imread(imname)
    return mahotas.features.haralick(img).mean(0)

def features_for_glob(glob, new_filename):
    with open(new_filename, 'w') as file: 
        for img in glob:                   
            feature = features_for(img)
            for f in feature:
                file.write(str(f) + ",")
            else:
                file.write(str(f) + "\n")

def main():

    cats = glob('E:/Uczelnia/sem6/BIAI/train/cats/*.jpg')[:10]
    dogs = glob('E:/Uczelnia/sem6/BIAI/train/dogs/*.jpg')[:10]
    unlabeled = glob('E:/Uczelnia/sem6/BIAI/test1/test1/*.jpg')[:10]
    
    print("Extracting from cats")
    features_for_glob(cats, "cats_features.csv")
    print("Extracting from dogs")
    features_for_glob(dogs, "dogs_features.csv")
    print("Extracting from unlabeled")
    features_for_glob(unlabeled, "unlabeled_features.csv")



if __name__ == '__main__':
    start_time = time.time();
    main()
    print("All done in %s seconds" % (time.time() - start_time))

