package com.devshorts.enumerable.iterators;

import java.util.HashSet;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.Queue;

public class DistinctUnion<TSource> extends EnumerableIterator<TSource> {
    private final Iterable<TSource> intersectWith;

    private Queue<TSource> list;

    public DistinctUnion(Iterable<TSource> input, Iterable<TSource> intersectWith) {
        super(input);
        this.intersectWith = intersectWith;
    }

    @Override
    public boolean hasNext(){
        if(list == null){
            buildSet();
        }

        return !list.isEmpty();
    }

    @Override
    public TSource next(){
        return list.remove();
    }

    private void buildSet() {
        if(list == null){
            list = new LinkedList<>();
        }
        else{
            return;
        }

        HashSet<TSource> set = new HashSet<>();

        Iterator<TSource> it = intersectWith.iterator();

        while(source.hasNext() || it.hasNext()){
            if(source.hasNext()){
                set.add(source.next());
            }
            if(it.hasNext()){
                set.add(it.next());
            }
        }

        list.addAll(set);
    }
}
