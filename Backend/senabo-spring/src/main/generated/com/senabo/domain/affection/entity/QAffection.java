package com.senabo.domain.affection.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QAffection is a Querydsl query type for Affection
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QAffection extends EntityPathBase<Affection> {

    private static final long serialVersionUID = 1414690046L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QAffection affection = new QAffection("affection");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    public final NumberPath<Integer> changeAmount = createNumber("changeAmount", Integer.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    public final NumberPath<Integer> score = createNumber("score", Integer.class);

    public final EnumPath<com.senabo.common.ActivityType> type = createEnum("type", com.senabo.common.ActivityType.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QAffection(String variable) {
        this(Affection.class, forVariable(variable), INITS);
    }

    public QAffection(Path<? extends Affection> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QAffection(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QAffection(PathMetadata metadata, PathInits inits) {
        this(Affection.class, metadata, inits);
    }

    public QAffection(Class<? extends Affection> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

