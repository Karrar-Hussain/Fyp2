function  d= icaVisualizeFunc(ori,ica)
d=1;
p1=plot(ori);
hold on;
p2=plot(ica);
hold on;
legend([p1,p2],'Original','ICA');