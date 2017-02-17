function  d= pcaVisualizeFunc(ori,pca)
d=1;
p1=plot(ori);
hold on;
p2=plot(pca);
hold on;
legend([p1,p2],'Original','PCA');