from glob import glob
import mahotas
import mahotas.features
import milk
import numpy as np
from jug import TaskGenerator


#@TaskGenerator
#def features_for(imname):
#    img = mahotas.imread(imname)
#    return mahotas.features.haralick(img).mean(0)

#@TaskGenerator
#def learn_model(features, labels):
#    learner = milk.defaultclassifier()
#    return learner.train(features, labels)

#@TaskGenerator
#def classify(model, features):
#     return model.apply(features)



#positives = glob('E:/Uczelnia/sem6/BIAI/train/dogs/*.jpg')
#negatives = glob('E:/Uczelnia/sem6/BIAI/train/cats/*.jpg')
#unlabeled = glob('E:/Uczelnia/sem6/BIAI/train/dogs/*.jpg')

#features = map(features_for, negatives + positives)
#labels = [0] * len(negatives) + [1] * len(positives)

#model = learn_model(features, labels)

#labeled = [classify(model, features_for(u)) for u in unlabeled[:1]]


#print("test")

features = np.random.randn(100,20)
features[:50] *= 2
labels = np.repeat((0,1), 50)

classifier = milk.defaultclassifier()
model = classifier.train(features, labels)
new_label = model.apply(np.random.randn(100))
new_label2 = model.apply(np.random.randn(100)*2)
