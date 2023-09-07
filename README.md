# Ants_Unity
Using unity, the objective is to model an ant colony.

Ant project uses the NeuralNetwork nuget package to build their brain based on a genome.
An ant can : 
  - Catch food
  - Drop food at their nest
  - Drop pheromone whose type depends if the ant is carrying food or not.
  - Scan its environment to measure :
      + Pheromone densities
      + Obstacle distances
      + Food token within its vision field
  - Use scanning result to make its next choice

Ants are then sort by their performance during their lifetime. This performance function should be carefully chosen, due to its impact on evolution trail.
Best ones are allowed to reproduce, following biological process of cross-over / mutations


And here we go again with this new generation
