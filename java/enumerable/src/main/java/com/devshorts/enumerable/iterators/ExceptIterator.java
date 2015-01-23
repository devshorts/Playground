package com.devshorts.enumerable.iterators;

import java.util.HashSet;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.Queue;

public class ExceptIterator<TSource> extends EnumerableIterator<TSource> {
    private final Iterable<TSource> exceptWith;

    private Queue<TSource> list;

    public ExceptIterator(Iterable<TSource> input, Iterable<TSource> exceptWith) {
        super(input);
        this.exceptWith = exceptWith;
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

        while(source.hasNext()){
            set.add(source.next());
        }

        for(TSource n : exceptWith){
            set.remove(n);
        }

        list.addAll(set);
    }
}
