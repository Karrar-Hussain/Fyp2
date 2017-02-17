function [d] = myfunc(ori,pca,ica)
[d,L] = wavedec(right,4,'coif5');
p1=plot(ori);
hold on;
p2=plot(pca)
hold on;
p3=plot(ica)
hold on;
legend([p1,p2,p3],'Original','PCA','ICA')
dlmwrite('my_lenth.out',L,';');
dlmwrite('my_data.out',d,';');