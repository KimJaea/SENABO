package com.senabo.domain.bath.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QBath is a Querydsl query type for Bath
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QBath extends EntityPathBase<Bath> {

    private static final long serialVersionUID = 475893342L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QBath bath = new QBath("bath");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QBath(String variable) {
        this(Bath.class, forVariable(variable), INITS);
    }

    public QBath(Path<? extends Bath> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QBath(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QBath(PathMetadata metadata, PathInits inits) {
        this(Bath.class, metadata, inits);
    }

    public QBath(Class<? extends Bath> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

