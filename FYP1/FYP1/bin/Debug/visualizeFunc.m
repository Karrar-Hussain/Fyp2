function  d= visualizeFunc(ori,pca,ica)
d=1;
p1=plot(ori);
hold on;
p2=plot(pca);
hold on;
p3=plot(ica);
hold on;
legend([p1,p2,p3],'Original','PCA','ICA');