##
## Imports
##
#import numpy as np
#import csv

## Transfer functions
#def sgm(x, Derivative=False):
#    if not Derivative:
#        return 1 / (1 + np.exp(-x))
#    else:
#        out = sgm(x)
#        return out * (1 - out)

#def linear(x, Derivative=False):
#    if not Derivative:
#        return x
#    else:
#        return 1.0

#def gaussian(x, Derivative=False):
#     if not Derivative:
#        return np.exp(-x ** 2)
#     else:
#        return -2 * x * np.exp(-x ** 2)

#def tanh(x, Derivative=False):
#    if not Derivative:
#        return np.tanh(x)
#    else:
#        return 1.0 - np.tanh(x) ** 2

##
## Classes
##
#class BackPropagationNetwork:
#    """The back-propagation network"""

#    #
#    #Class members
#    #
#    layerCount = 0
#    shape = None
#    weights = [] #its matrix
#    tFuncs = []
#    #
#    #Class methods
#    #
#    def __init__(self, layerSize, layerFunctions=None):
#        """Initialize the network"""

#        # Layer info
#        self.layerCount = len(layerSize) - 1
#        self.shape = layerSize

#        if layerFunctions is None:
#            lFuncs = []
#            for i in range(self.layerCount):
#                if i == self.layerCount - 1:
#                    lFuncs.append(linear)
#                else:
#                    lFuncs.append(sgm)
#        else:
#            if len(layerSize) != len(layerFunctions):
#                raise ValueError("INcompatible list of transfer functions.")
#            elif layerFunctions[0] is not None:
#                raise ValueError("INput layer cannot have a transfer function.")
#            else:
#                lFuncs = layerFunctions[1:]

#        self.tFuncs = lFuncs
#        # Data from last Run
#        self._layerInput = []
#        self._layerOutput = []
#        self._perviousWeightDelta = []

#        # Create the weight arrays
#        for(l1, l2) in zip(layerSize[:-1], layerSize[1:]):
#            self.weights.append(np.random.normal(scale =0.1, size = (l2, l1 + 1)))
#            self._perviousWeightDelta.append(np.zeros((l2, l1 + 1)))

#    # Run method

#    def Run(self, input):
#        """Run the network based on oinput data"""

#        lnCases = input.shape[0]

#        # Clear out the previus intermediate value lists
#        self._layerInput = []
#        self._layerOutput = []

#        # Run it!
#        for index in range(self.layerCount):
#            #Determine layer input
#            if index == 0:
#                layerInput = self.weights[0].dot(np.vstack([input.T, np.ones([1, lnCases])]))
#            else:
#                layerInput = self.weights[index].dot(np.vstack([self._layerOutput[-1], np.ones([1, lnCases])]))
            
#            self._layerInput.append(layerInput)
#            self._layerOutput.append(self.tFuncs[index](layerInput))
#        return self._layerOutput[-1].T

#    #TrainEpoch method

#    def TrainEpoch(self, input, target, trainingRate=0.2, momentum=0.5):
#        """This method trains the netwokr for one epoch"""

#        delta = []
#        lnCases = input.shape[0]

#        # First run the network
#        self.Run(input)

#        #Calculate out deltas
#        for index in reversed(range(self.layerCount)):
#            if index == self.layerCount - 1:
#                # Compare to the target values
#                output_delta = self._layerOutput[index] - target.T
#                error = np.sum(output_delta ** 2)
#                delta.append(output_delta * self.tFuncs[index](self._layerInput[index], True))
#            else:
#                # Compare to the following layer's delta
#                delta_pullback = self.weights[index + 1].T.dot(delta[-1])
#                delta.append(delta_pullback[:-1, :] * self.tFuncs[index](self._layerInput[index], True))
        
#        # Compute weight deltas
#        for index in range(self.layerCount):
#             delta_index = self.layerCount - 1 - index

#             if index == 0:
#                 layerOutput = np.vstack([input.T, np.ones([1, lnCases])])
#             else:
#                 layerOutput = np.vstack([self._layerOutput[index - 1], np.ones([1, self._layerOutput[index - 1].shape[1]])])
                
#             curWeightDelta = np.sum(\
#                                 layerOutput[None,:,:].transpose(2,0,1) * delta[delta_index][None,:,:].transpose(2,1,0)\
#                                , axis = 0)

#             weightDelta = trainingRate * curWeightDelta + momentum * self._perviousWeightDelta[index]

#             self.weights[index] -=  weightDelta

#             self._perviousWeightDelta[index] = weightDelta
#        return error



## If run as a script, create a test object
#if __name__ == "__main__":
    
#    data = []
#    tab = []

#    for i in range(-20000, 10):
#        for j in range(-10, 10):
#            tab.append([i,j])
#    #    data.append([i, i % 2])

#    #with open('test.csv', 'w') as file:
#    #    writter = csv.writer(file)
#    #    writter.writerows(data);
    
#    bpn = BackPropagationNetwork((2,2,1), [None, sgm, linear])
#    #for i in range(1):
#    #    print 'iteration ' + str(i)
#    #    with open('test.csv', 'rb') as f:
#    #        reader = csv.reader(f)
#    #        for row in reader:   
#    #            for j in range(1):          
#    #                bpn.TrainEpoch(np.array([int(row[0])]),np.array([int(row[1])]) , momentum = 0.1)      
    
#    for it in tab:
#        bpn.TrainEpoch(np.array([it]), np.array([int(it[0] + it[1] > 0)]))     
#    ##print(bpn.shape)
#    ##print bpn.weights

#    #lvInput = np.array([[-1],[1], [4], [6], [-8]])
#    #lvTarget = np.array([[0], [1], [1], [1], [0]])
#    #lFuncs = [None, sgm]

    

#    #lnMax = 10000#0
#    #lnErr = 1e-5
#    #for i in range(lnMax + 1):
#    #    err = bpn.TrainEpoch(lvInput, lvTarget, momentum = 0.1)
#    #    if i % 2500 == 0:
#    #        print("Iteration {0}\tError: {1:0.6f}".format(i,err))
#    #    if err <= lnErr:
#    #        print("Minimum error reached at iteration {0}".format(i))
#    #        break

#    lvRun = np.array([[27, -14], [12, -25], [12, 45], [32423, -234]])
#    lvOutput = bpn.Run(lvRun)
#    temp = []
#    ileOk = 0
#    #for item in range(0,len(lvOutput)):          
#    #    if item % 2 == round(lvOutput[item]):
#    #        ileOk += 1
#    #    temp.append([item, lvOutput[item]])

#    #with open('test_output1x20kx10.csv', 'w') as file:  
#    #    writter = csv.writer(file)   
#    #    writter.writerows(temp)
        

#    print ileOk
#    # Display output
#    #for i in range(lvRun.shape[0]):
#    #    print("Input: {0} Output: {1}".format(lvRun[i], round(lvOutput[i])))
