package com.senabo.domain.communication.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QCommunication is a Querydsl query type for Communication
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QCommunication extends EntityPathBase<Communication> {

    private static final long serialVersionUID = -681946658L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QCommunication communication = new QCommunication("communication");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    public final EnumPath<com.senabo.common.ActivityType> type = createEnum("type", com.senabo.common.ActivityType.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QCommunication(String variable) {
        this(Communication.class, forVariable(variable), INITS);
    }

    public QCommunication(Path<? extends Communication> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QCommunication(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QCommunication(PathMetadata metadata, PathInits inits) {
        this(Communication.class, metadata, inits);
    }

    public QCommunication(Class<? extends Communication> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

