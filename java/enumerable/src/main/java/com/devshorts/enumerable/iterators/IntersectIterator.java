package com.devshorts.enumerable.iterators;

import java.util.HashSet;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.Queue;

public class IntersectIterator<TSource> extends EnumerableIterator<TSource> {
    private final Iterable<TSource> intersectWith;

    private Queue<TSource> list;

    public IntersectIterator(Iterable<TSource> input, Iterable<TSource> intersectWith) {
        super(input);
        this.intersectWith = intersectWith;
    }

    @Override
    public boolean hasNext(){
        if(list == null){
            intersect();
        }

        return !list.isEmpty();
    }

    @Override
    public TSource next(){
        return list.remove();
    }

    private void intersect() {
        if(list == null){
            list = new LinkedList<>();
        }
        else{
            return;
        }

        HashSet<TSource> set = new HashSet<>();

        for (TSource anIntersectWith : intersectWith) {
            set.add(anIntersectWith);
        }

        while(source.hasNext()){
            TSource n = source.next();
            if(set.contains(n)){
                list.add(n);
                set.remove(n);
            }
        }
    }
}
