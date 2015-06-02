#import numpy as np
#import cv2
#import os
##http://opencv-python-tutroals.readthedocs.org/en/latest/py_tutorials/py_imgproc/py_contours/py_contours_begin/py_contours_begin.html
#def prepare_single_file(directory,filename, show):
#    img = cv2.imread(os.path.normcase(directory + "//" + filename), 0)


#def perform(file):
#    im = cv2.imread(file)
#    imgray = cv2.cvtColor(im,cv2.COLOR_BGR2GRAY)
#    ret,thresh = cv2.threshold(imgray,127,255,0)
#    image, contours, hierarchy = cv2.findContours(thresh,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
#    img = cv2.drawContours(im, contours, -1, (0,255,0), 3) #bgr
#    return img


#def display(bmp):
#    cv2.imshow('img',bmp)
#    cv2.waitKey(0)
#    cv2.destroyAllWindows()


##Main
#directory = r'E:/Uczelnia/sem6/BIAI/train/dogs'

#for file in os.listdir(directory):       
#    img = perform(os.path.normcase(directory + "//" + file))
#    display(img)
